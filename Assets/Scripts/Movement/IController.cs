public interface IController<T>
{
    void Control(T supportingValue);
}

public interface IController
{
    void Control();
}
