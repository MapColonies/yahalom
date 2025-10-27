
namespace com.mapcolonies.yahalom.Redux
{
    public class DynamicStateActions
    {
        public record AddSubState(string Key, object State)
        {
            public string Key { get; } = Key;
            public object State { get; } = State;
        }
    }
}
