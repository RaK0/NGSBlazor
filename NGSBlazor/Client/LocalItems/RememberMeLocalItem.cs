using NGSBlazor.Client.Interfaces;

namespace NGSBlazor.Client.LocalItems
{
    public class RememberMeLocalItem : ILocalItem
    {
        public bool? Value { get; set; }

        string ILocalItem.Name => "RememberMe";
    }
}
