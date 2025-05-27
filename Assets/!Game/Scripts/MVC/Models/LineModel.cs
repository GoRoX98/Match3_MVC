public class LineModel : Model
{
    private int _id;
    public int LineId => _id;
    
    public LineModel(LineView view, int id) : base(view)
    {
        _id = id;
    }
}
