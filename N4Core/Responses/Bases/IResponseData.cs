namespace N4Core.Responses.Bases
{
    public interface IResponseData<out TResponseType>
    {
        public TResponseType Data { get; }
    }
}
