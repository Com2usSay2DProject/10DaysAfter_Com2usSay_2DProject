using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// generates a mesh for the referenced mesh filter which has a full quad for every cell of the map<br/>
    /// for example used in DebugTerrain of the Three demo for its building grid and mesh overlays
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/views">https://citybuilder.softleitner.com/manual/views</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_mesh_overlay_generator.html")]
    public class MeshOverlayGenerator : MonoBehaviour
    {
        [Tooltip("a mesh with a full quad for every cell of the map gets generated and set on this filter on script start")]
        public MeshFilter Filter;
        [Tooltip("check to apply map height to the mesh vertices")]
        public bool ApplyHeight;
        [Tooltip("distribute uvs from 0 to 1 across the whole mesh instead of each quad, vertices are reused across quads")]
        public bool StretchUVs;

        private void Start()
        {
            Filter.mesh = GenerateMesh(ApplyHeight, StretchUVs);
        }

        public static Mesh GenerateMesh(bool applyHeight, bool stretchUVs)
        {
            var mesh = new Mesh();

            var map = Dependencies.Get<IMap>();
            var gridPositions = Dependencies.Get<IGridPositions>();
            var gridHeights = applyHeight ? Dependencies.GetOptional<IGridHeights>() : null;

            var verts = new List<Vector3>();
            var tris = new List<int>();
            var uvs = new List<Vector2>();

            var size = map.Size;
            var offset = map.CellOffset;

            var isXY = map.IsXY;

            if (stretchUVs)
            {
                Vector3 p, a;

                for (int y = 0; y <= size.y; y++)
                {
                    for (int x = 0; x <= size.x; x++)
                    {
                        p = gridPositions.GetWorldPosition(new Vector2Int(x, y));

                        if (isXY)
                        {
                            a = p;

                            if (gridHeights != null)
                            {
                                a.z = gridHeights.GetHeight(a);
                            }
                        }
                        else
                        {
                            a = p;

                            if (gridHeights != null)
                            {
                                a.y = gridHeights.GetHeight(a);
                            }
                        }

                        verts.Add(a);

                        uvs.Add(new Vector2((float)x / size.x, (float)y / size.y));
                    }
                }

                for (int y = 0; y < size.y; y++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        //bottom left index of the quad
                        var rb = x + (size.x + 1) * y;
                        //top left index of the quad
                        var rt = x + (size.x + 1) * (y + 1);

                        tris.Add(rb);
                        tris.Add(rt);
                        tris.Add(rb + 1);
                        tris.Add(rt);
                        tris.Add(rt + 1);
                        tris.Add(rb + 1);
                    }
                }
            }
            else
            {
                Vector3 p, a, b, c, d;
                var i = 0;

                for (int y = 0; y < size.y; y++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        p = gridPositions.GetWorldPosition(new Vector2Int(x, y));

                        if (isXY)
                        {
                            a = p;
                            b = p + new Vector3(offset.x, 0f, 0f);
                            c = p + new Vector3(0f, offset.y, 0f);
                            d = p + new Vector3(offset.x, offset.y, 0f);

                            if (gridHeights != null)
                            {
                                a.z = gridHeights.GetHeight(a);
                                b.z = gridHeights.GetHeight(b);
                                c.z = gridHeights.GetHeight(c);
                                d.z = gridHeights.GetHeight(d);
                            }
                        }
                        else
                        {
                            a = p;
                            b = p + new Vector3(offset.x, 0f, 0f);
                            c = p + new Vector3(0f, 0f, offset.z);
                            d = p + new Vector3(offset.x, 0f, offset.z);

                            if (gridHeights != null)
                            {
                                a.y = gridHeights.GetHeight(a);
                                b.y = gridHeights.GetHeight(b);
                                c.y = gridHeights.GetHeight(c);
                                d.y = gridHeights.GetHeight(d);
                            }
                        }

                        verts.Add(a);
                        verts.Add(b);
                        verts.Add(c);
                        verts.Add(d);

                        tris.Add(i * 4 + 0);
                        tris.Add(i * 4 + 2);
                        tris.Add(i * 4 + 1);
                        tris.Add(i * 4 + 2);
                        tris.Add(i * 4 + 3);
                        tris.Add(i * 4 + 1);

                        uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));
                        uvs.Add(new Vector2(1, 1));

                        i++;
                    }
                }
            }

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);

            return mesh;
        }
    }
}
