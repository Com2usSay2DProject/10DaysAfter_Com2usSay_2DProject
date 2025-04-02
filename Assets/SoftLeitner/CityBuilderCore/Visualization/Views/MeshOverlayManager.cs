using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// implementation of <see cref="IOverlayManager"/> that visualizes values by setting vertex colors
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/views">https://citybuilder.softleitner.com/manual/views</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_mesh_overlay_manager.html")]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshOverlayManager : MonoBehaviour, IOverlayManager
    {
        [Tooltip("gets enabled when overlays are displayed, values are visualized by setting vertex colors on the mesh(one quad per point, see code for details)")]
        public MeshRenderer Renderer;
        [Tooltip("optional visualizer that explains how the layer value on a point are calculated when a LayerView is active")]
        public LayerKeyVisualizer LayerKeyVisualizer;
        [Tooltip("optional visualizer that shows the numerical connection value of the map point under the mouse when a ConnectionView is active")]
        public ConnectionValueVisualizer ConnectionValueVisualizer;
        [Tooltip("check if the target MeshOverlayGenerator has StretchUVs checked, changes how indices are calculated")]
        public bool StretchUVs;

        private MeshRenderer _renderer;

        private ViewEfficiency _currentEfficiencyView;
        private ViewLayer _currentLayerView;
        private ViewConnection _currentConnectionView;

        private IMap _map;
        private ILayerManager _layerManager;
        private IConnectionManager _connectionManager;

        private float[] _values;
        private Color[] _colors;

        protected virtual void Awake()
        {
            Dependencies.Register<IOverlayManager>(this);
        }

        protected virtual void Start()
        {
            _renderer = GetComponent<MeshRenderer>();

            _layerManager = Dependencies.GetOptional<ILayerManager>();
            if (_layerManager != null)
                _layerManager.Changed += layerChanged;

            _connectionManager = Dependencies.GetOptional<IConnectionManager>();
            if (_connectionManager != null)
                _connectionManager.Changed += connectionChanged;

            _map = Dependencies.Get<IMap>();

            if (StretchUVs)
            {
                _values = new float[(_map.Size.x + 1) * (_map.Size.y + 1)];
                _colors = new Color[(_map.Size.x + 1) * (_map.Size.y + 1)];
            }
            else
            {
                _colors = new Color[_map.Size.x * _map.Size.y * 4];
            }

            resetRenderer();
        }

        public void ActivateOverlay(ViewLayer view)
        {
            var range = view.Maximum - view.Minimum;
            var bottom = -view.Minimum;

            foreach (var value in Dependencies.Get<ILayerManager>().GetValues(view.Layer))
            {
                setValue(value.Item1, (float)(value.Item2 + bottom) / range, view.Gradient);
            }

            setRenderer();

            _currentLayerView = view;

            if (LayerKeyVisualizer)
                LayerKeyVisualizer.Activate(view.Layer);
        }

        public void ActivateOverlay(ViewConnection view)
        {
            var range = view.Maximum - view.Minimum;
            var bottom = -view.Minimum;

            foreach (var value in view.GetValues())
            {
                setValue(value.Key, (float)(value.Value + bottom) / range, view.Gradient);
            }

            setRenderer();

            _currentConnectionView = view;

            if (ConnectionValueVisualizer)
                ConnectionValueVisualizer.Activate(view.Connection);
        }

        public void ActivateOverlay(ViewEfficiency view)
        {
            _currentEfficiencyView = view;

            refreshEfficiencyOverlay();
            this.StartChecker(refreshEfficiencyOverlay);
        }

        public void ClearOverlay()
        {
            StopAllCoroutines();

            resetRenderer();

            _currentLayerView = null;
            _currentConnectionView = null;
            _currentEfficiencyView = null;

            if (_values != null)
                Array.Clear(_values, 0, _values.Length);

            if (LayerKeyVisualizer)
                LayerKeyVisualizer.Deactivate();

            if (ConnectionValueVisualizer)
                ConnectionValueVisualizer.Deactivate();
        }

        private void setValue(Vector2Int point, float value, Gradient gradient)
        {
            var index = point.y * _map.Size.x + point.x;
            var color = gradient.Evaluate(value);

            if (StretchUVs)
            {
                var size = _map.Size;
                var x = point.x;
                var y = point.y;

                //bottom left index of the quad
                var rb = x + (size.x + 1) * y;
                //top left index of the quad
                var rt = x + (size.x + 1) * (y + 1);
                
                //keep highest value when set more than once from bordering points
                void set(int i)
                {
                    if (Mathf.Abs(_values[i]) < Mathf.Abs(value))
                    {
                        _values[i] = value;
                        _colors[i] = color;
                    }
                }

                set(rb);
                set(rb + 1);
                set(rt);
                set(rt + 1);
            }
            else
            {
                _colors[index * 4 + 0] = color;
                _colors[index * 4 + 1] = color;
                _colors[index * 4 + 2] = color;
                _colors[index * 4 + 3] = color;
            }
        }
        private void setRenderer()
        {
            Renderer.GetComponent<MeshFilter>().mesh.SetColors(_colors);
            Renderer.enabled = true;
        }
        private void resetRenderer()
        {
            Renderer.enabled = false;
            Array.Clear(_colors, 0, _colors.Length);
        }

        private void layerChanged(Layer layer)
        {
            if (_currentLayerView && _currentLayerView.Layer == layer)
                refreshLayerOverlay();
        }
        private void refreshLayerOverlay()
        {
            var current = _currentLayerView;
            ClearOverlay();
            _currentLayerView = current;
            ActivateOverlay(_currentLayerView);
        }

        private void connectionChanged(Connection connection)
        {
            if (_currentConnectionView && _currentConnectionView.Connection == connection)
                refreshConnectionOverlay();
        }
        private void refreshConnectionOverlay()
        {
            var current = _currentConnectionView;
            ClearOverlay();
            _currentConnectionView = current;
            ActivateOverlay(current);
        }

        private void refreshEfficiencyOverlay()
        {
            if (_values != null)
                Array.Clear(_values, 0, _values.Length);

            var buildingManager = Dependencies.Get<IBuildingManager>();
            foreach (var building in buildingManager.GetBuildings().Where(b => b.HasBuildingPart<IEfficiencyFactor>()))
            {
                var efficiency = building.Efficiency;
                foreach (var point in PositionHelper.GetStructurePositions(building.Point, building.Size))
                {
                    setValue(point, efficiency, _currentEfficiencyView.Gradient);
                }
            }

            setRenderer();
        }
    }
}