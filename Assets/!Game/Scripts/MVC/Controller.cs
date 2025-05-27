public abstract class Controller
{
    protected Model _model;
    protected View _view;
    
    public Controller(Model model, View view)
    {
        _model = model;
        _view = view;
    }
}
