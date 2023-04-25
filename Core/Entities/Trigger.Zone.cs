namespace Pladi.Core.Entities
{
    public class ZoneTrigger : Trigger
    {
        // [constructors]

        public ZoneTrigger()
        {
            Width = 48f;
            Height = 48f;
        }

        public ZoneTrigger(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}