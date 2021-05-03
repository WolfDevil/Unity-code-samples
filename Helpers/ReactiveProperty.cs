using System;

public class ReactiveProperty<TData>
{
    private string _id;

    public TData Value
    {
        get => _data;
        private set => _data = value;
    }

    public event Action<TData> OnChanged = data => { };

    private TData _data;

    public ReactiveProperty(TData data, string id = "")
    {
        _id = string.IsNullOrEmpty(id) ? this.GetHashCode().ToString() : id;
        Value = data;
    }

    public void Set(TData data)
    {
        if (Equals(_data, data))
        {
            return;
        }

        _data = data;
        OnChanged(data);
    }
}

public class ReactiveProperty<TData1, TData2>
{
    private string _id;
    private TData1 _data1;
    private TData2 _data2;

    public TData1 Value1
    {
        get => _data1;
        private set => _data1 = value;
    }

    public TData2 Value2
    {
        get => _data2;
        private set => _data2 = value;
    }

    public event Action<TData1, TData2> OnChanged = (data1, data2) => { };


    public ReactiveProperty(TData1 data1, TData2 data2, string id = "")
    {
        _id = string.IsNullOrEmpty(id) ? this.GetHashCode().ToString() : id;
        Value1 = data1;
        Value2 = data2;
    }

    public void Set(TData1 data1, TData2 data2)
    {
        Value1 = data1;
        Value2 = data2;

        OnChanged(_data1, _data2);
    }
}