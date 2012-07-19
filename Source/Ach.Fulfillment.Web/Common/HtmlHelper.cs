namespace Ach.Fulfillment.Web.Common
{
    using System;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class HtmlHelperExtended
    {
        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, System.Linq.Expressions.Expression<Func<TModel, TValue>> expression)
        {
            var label = html.LabelFor(expression);

            var required = label.ToHtmlString().Replace("<label", @"<label class=""required""");

            return MvcHtmlString.Create(required);
        }

        public static MvcHtmlString ActionImage(this HtmlHelper html, string controller, string action, object routeValues, string imagePath, string alt)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            var imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, controller, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            var anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

    }
}