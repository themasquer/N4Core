using Microsoft.AspNetCore.Mvc.Rendering;
using N4Core.Types.Extensions;

namespace N4Core.Services.Models
{
    public class PageOrderModel : PageModel
    {
        public string? OrderExpression { get; set; }
        public List<SelectListItem>? OrderExpressions { get; set; }

        protected bool _orderDirectionDescending;
        public bool OrderDirectionDescending 
        {
            get
            {
                return (OrderExpression ?? string.Empty).EndsWith("Desc") ? true : _orderDirectionDescending;
            }
            set
            {
                _orderDirectionDescending = value;
            }
        }

        protected List<string>? _orderExpressionsForEntityProperties;

        /// <summary>
        /// Must be assigned by related entity property names.
        /// Turkish characters will be replaced with relevant English characters.
        /// </summary>
        public List<string>? OrderExpressionsForEntityProperties
        {
            get
            {
                return _orderExpressionsForEntityProperties;
            }
            set
            {
                _orderExpressionsForEntityProperties = value ?? new List<string>();
                OrderExpressions = new List<SelectListItem>();
                foreach (var orderExpression in _orderExpressionsForEntityProperties)
                {
                    OrderExpressions.Add(new SelectListItem(orderExpression, orderExpression.ChangeTurkishCharactersToEnglish().Replace(" ", "")));
                    OrderExpressions.Add(new SelectListItem(orderExpression + " Azalan", orderExpression.ChangeTurkishCharactersToEnglish().Replace(" ", "") + "Desc"));
                }
                if (OrderExpressions.Any())
                {
                    OrderExpression = string.IsNullOrWhiteSpace(OrderExpression) ? OrderExpressions.First().Value : OrderExpression;
                }
            }
        }

        public PageOrderModel() : base()
        {
            OrderExpression = string.Empty;
            _orderExpressionsForEntityProperties = new List<string>();
            OrderExpressions = new List<SelectListItem>();
        }
    }
}
