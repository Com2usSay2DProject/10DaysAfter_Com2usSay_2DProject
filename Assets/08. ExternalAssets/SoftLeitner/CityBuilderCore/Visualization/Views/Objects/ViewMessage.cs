using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// view that sends messages to a gameobject when activated and deactivated
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/views">https://citybuilder.softleitner.com/manual/views</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_view_message.html")]
    [CreateAssetMenu(menuName = "CityBuilder/Views/" + nameof(ViewMessage))]
    public class ViewMessage : View
    {
        [Tooltip("name of the gameobject that will get have all renderers on it enabled while the view is active")]
        public string ObjectName;

        [Tooltip("message that may be sent when the view becomes active")]
        public string ActivateMessage;
        [Tooltip("message that may be sent when the view becomes inactive")]
        public string DeactivateMessage;

        public override void Activate()
        {
            if (!string.IsNullOrWhiteSpace(ActivateMessage))
                GameObject.Find(ObjectName).SendMessage(ActivateMessage);
        }
        public override void Deactivate()
        {
            if (!string.IsNullOrWhiteSpace(DeactivateMessage))
                GameObject.Find(ObjectName).SendMessage(DeactivateMessage);
        }
    }
}