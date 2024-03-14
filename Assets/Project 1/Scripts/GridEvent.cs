using Events;

public class GridEvent : Event<GridEvent>
{
    public int Index1, Index2;
    
    public static GridEvent Get(int i1, int i2)
    {
        var evt = GetPooledInternal();
        evt.Index1 = i1;
        evt.Index2 = i2;

        return evt;
    }
}