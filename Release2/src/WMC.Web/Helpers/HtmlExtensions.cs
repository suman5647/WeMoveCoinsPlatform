using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace WMC.Helpers
{
    public static class HtmlExtensions
    {
        //private static readonly IValidatorFactory ValidatorFactory = new AttributedValidatorFactory();

        private static string GetElementName(string elementName, string modelPrefix, string prefixToReplace)
        {
            if (!String.IsNullOrEmpty(modelPrefix))
            {
                if (!String.IsNullOrEmpty(prefixToReplace))
                    return elementName.Replace(prefixToReplace, modelPrefix);
                else
                    return modelPrefix + "." + elementName;
            }

            return elementName;
        }

        public static MvcHtmlString NgValidationMessageFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string modelPrefix = "", string prefixToReplace = "")
        {
            var elementName = GetElementName(ExpressionHelper.GetExpressionText(expression), modelPrefix, prefixToReplace);

            string observedFieldId = elementName.Replace('.', '-');
            string elementId = observedFieldId + "-ValidationMessage";

            return new MvcHtmlString("<span class=\"error validation-message\" id=\"" + elementId + "\">This is invalid</span>");
        }

        public static MvcHtmlString NgCheckBoxFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, string modelPrefix = "", string prefixToReplace = "", bool renderNgInit = false, object _htmlAttributes = null)
        {
            IDictionary<string, object> htmlAttributes = BuildHtmlAttributes(html, expression, modelPrefix, prefixToReplace, renderNgInit, _htmlAttributes);
            htmlAttributes.Add("data-validated", "");
            var result = html.CheckBoxFor(expression, htmlAttributes);
            result = ApplyAttributeCleanup(result);
            return result;
        }

        public static MvcHtmlString NgRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, object value = null, string modelPrefix = "", string prefixToReplace = "", bool renderNgInit = false, object _htmlAttributes = null)
        {
            IDictionary<string, object> htmlAttributes = BuildHtmlAttributes(html, expression, modelPrefix, prefixToReplace, renderNgInit, _htmlAttributes);
            htmlAttributes.Add("data-validated", "");

            var result = html.RadioButtonFor(expression, value, htmlAttributes);
            result = ApplyAttributeCleanup(result);
            return result;
        }

        public static MvcHtmlString NgDisplayFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return NgDisplayFor(html, expression, "span");
        }

        public static MvcHtmlString NgDisplayFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string wrapperTag, bool renderNgInit = false)
        {
            //var id = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));

            IDictionary<string, object> htmlAttributes = BuildHtmlAttributes(html, expression, renderNgInit: renderNgInit);
            string ngModelName = htmlAttributes.Where(a => a.Key == "data-ng-model").First().Value.ToString();

            StringBuilder attributeString = new StringBuilder();
            foreach (KeyValuePair<string, object> attribute in htmlAttributes)
                attributeString.Append(attribute.Key + "=\"" + attribute.Value + "\" ");
            return MvcHtmlString.Create(string.Format("<{0} {1}>{2}</{0}>", wrapperTag, attributeString, "{{" + ngModelName + "}}"));
        }

        public static MvcHtmlString NgListBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList, string modelPrefix = "", string prefixToReplace = "", bool renderNgInit = false)
        {
            IDictionary<string, object> htmlAttributes = BuildHtmlAttributes(html, expression, modelPrefix, prefixToReplace, renderNgInit);
            return html.ListBoxFor(expression, selectList, htmlAttributes);
        }

        public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            string angularAppName,
            string modelPrefix = "",
            string prefixToReplace = "",
            bool renderNgInit = false,
            object htmlAttributes = null)
        {
            IDictionary<string, object> mergedHtmlAttributes = BuildHtmlAttributes(html, expression, modelPrefix, prefixToReplace, renderNgInit, htmlAttributes);
            string jsonId = mergedHtmlAttributes["id"].ToString().Replace("-", "") + "json";

            mergedHtmlAttributes.Add("data-validated", "");
            mergedHtmlAttributes.Add("ng-options", "obj.Value as obj.Text for obj in " + jsonId);

            MvcHtmlString result = html.DropDownListFor(expression, selectList, mergedHtmlAttributes);
            result = ApplyAttributeCleanup(result);

            //now add the json output for angular binding
            MvcHtmlString js = new MvcHtmlString("<script>" +
                Environment.NewLine +
                angularAppName + "." + jsonId + " = '" + JsonConvert.SerializeObject(selectList) + "';" +
                Environment.NewLine +
                "</script>" +
                Environment.NewLine +
                Environment.NewLine);

            return new MvcHtmlString(js.ToHtmlString() + result.ToHtmlString());
        }

        public static MvcHtmlString NgTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string watermark = "", string modelPrefix = "", string prefixToReplace = "", bool renderNgInit = false, object htmlAttributes = null)
        {
            PropertyInfo propInfo = ((MemberExpression)expression.Body).Member as PropertyInfo;

            //define the model
            IDictionary<string, object> mergedHtmlAttributes = BuildHtmlAttributes(html, expression, modelPrefix, prefixToReplace, renderNgInit, htmlAttributes);

            //if this is a number make it a number input
            List<string> numTypes = new List<string>() { "Int16", "Int32", "Int64" };

            //if we have a watermark populate the placeholder attribute
            if (!String.IsNullOrEmpty(watermark))
            {
                string asterisk = "";
                if (mergedHtmlAttributes["required"] != null)
                    asterisk = "*";

                mergedHtmlAttributes.Add("placeholder", asterisk + watermark);
            }

            if (numTypes.Contains(propInfo.PropertyType.Name))
            {
                mergedHtmlAttributes.Add("type", "number");
                mergedHtmlAttributes.Add("ng-pattern", "/\\d+/");
                if (mergedHtmlAttributes["required"] == null)
                    mergedHtmlAttributes.Add("required", "");
            }

            mergedHtmlAttributes.Add("data-validated", "");

            var result = html.TextBoxFor(expression, mergedHtmlAttributes);
            result = ApplyAttributeCleanup(result);
            return result;
        }

        private static MvcHtmlString ApplyAttributeCleanup(MvcHtmlString result)
        {
            string hacks = result.ToString();
            hacks = hacks.Replace("proxy-validity=\"\"", "proxy-validity"); //cleaning proxy-validation tag
            hacks = hacks.Replace("data-validated=\"\"", "data-validated"); //just drop the directive
            hacks = hacks.Replace("required=\"\"", "required"); //cleaning required tag
            result = new MvcHtmlString(hacks);
            return result;
        }

        private static IDictionary<string, object> BuildHtmlAttributes<TModel, TValue>(HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string modelPrefix = "", string prefixToReplace = "", bool renderNgInit = false, object _htmlAttributes = null)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            PropertyInfo propInfo = memberExpression.Member as PropertyInfo;
            //List<ValidationConfig> configs = GetValidationConfigs(memberExpression.Expression.Type, propInfo);
            IDictionary<string, object> result = new Dictionary<string, object>();
            RouteValueDictionary attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(_htmlAttributes);

            string prefix = "";
            var elementName = GetElementName(ExpressionHelper.GetExpressionText(expression), modelPrefix, prefixToReplace);
            if (!String.IsNullOrEmpty(modelPrefix))
            {
                prefix = modelPrefix;
                if (!String.IsNullOrEmpty(prefixToReplace))
                    elementName = elementName.ReplaceFirst(prefixToReplace, prefix);
                else
                    elementName = prefix + "" + elementName;
            }

            result.Add("data-ng-model", elementName);
            result.Add("id", elementName.Replace('.', '-'));
            result.Add("name", elementName.Replace('.', '-'));

            if (renderNgInit)
            {
                Func<TModel, TValue> method = expression.Compile();
                TValue value = method(html.ViewData.Model);
                result.Add("data-ng-init", elementName + " = '" + value + "'");
            }

            //foreach (var vc in configs)
            //{
            //    switch (vc.ValidationType)
            //    {
            //        case ValidationTypes.RegularExpression:
            //            result.Add("ng-pattern", "/" + vc.Expression + "/");

            //            break;

            //        case ValidationTypes.GreatThan:
            //            result.Add("min", vc.ValueToCompare);
            //            break;

            //        case ValidationTypes.LessThan:
            //            result.Add("max", vc.ValueToCompare);
            //            break;

            //        case ValidationTypes.NotEmpty:
            //        case ValidationTypes.NotNull:
            //            if (!result.ContainsKey("required"))
            //                result.Add("required", "");
            //            break;
            //    }
            //}
            //Adding the htmlAttributes from the client
            result = MergeRouteValueDictionary(html, result, attributes);
            return result;
        }

        private static RouteValueDictionary MergeRouteValueDictionary<TModel>(HtmlHelper<TModel> html, IEnumerable<KeyValuePair<string, object>> value, RouteValueDictionary attributes)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();

            foreach (KeyValuePair<string, object> keyValuePair in value)
                routeValueDictionary.Add(keyValuePair.Key, keyValuePair.Value);

            if (attributes != null)
                foreach (KeyValuePair<string, object> keyValuePair in attributes)
                    routeValueDictionary.Add(keyValuePair.Key, keyValuePair.Value);

            return routeValueDictionary;
        }

        //private static List<ValidationConfig> GetValidationConfigs(Type model, PropertyInfo propInfo)
        //{
        //    List<ValidationConfig> result = new List<ValidationConfig>();
        //    IValidator val = ValidatorFactory.GetValidator(model);
        //    if (val != null)
        //    {
        //        var rules = val.ToList();
        //        foreach (var rule in rules)
        //        {
        //            var validators = rule.Validators.ToList();
        //            var property = rule as PropertyRule;

        //            //make sure we only get the rules that apply to this property!
        //            if (property.PropertyName.ToLower() == propInfo.Name.ToLower())
        //            {
        //                foreach (var propertyValidator in validators)
        //                {
        //                    switch (propertyValidator.GetType().Name)
        //                    {
        //                        case "NotEmptyValidator":
        //                        case "NotNullValidator":
        //                            //NotEmptyValidator gtv = propertyValidator as NotEmptyValidator;
        //                            result.Add(new ValidationConfig
        //                            {
        //                                ValidationType = ValidationTypes.NotEmpty,
        //                            });
        //                            break;

        //                        case "GreaterThanValidator":
        //                            GreaterThanValidator gtv = propertyValidator as GreaterThanValidator;
        //                            result.Add(new ValidationConfig
        //                            {
        //                                ValidationType = ValidationTypes.GreatThan,
        //                                ValueToCompare = gtv.ValueToCompare,
        //                            });
        //                            break;

        //                        case "LessThanOrEqualValidator":
        //                            LessThanOrEqualValidator ltoe = propertyValidator as LessThanOrEqualValidator;
        //                            result.Add(new ValidationConfig
        //                            {
        //                                ValidationType = ValidationTypes.LessThanOrEqualTo,
        //                                ValueToCompare = ltoe.MemberToCompare
        //                            });
        //                            break;

        //                        case "RegularExpressionValidator":
        //                            RegularExpressionValidator rev = propertyValidator as RegularExpressionValidator;
        //                            result.Add(new ValidationConfig
        //                            {
        //                                ValidationType = ValidationTypes.RegularExpression,
        //                                Expression = rev.Expression
        //                            });
        //                            break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        public static MvcHtmlString NgLabel2<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            MemberExpression member = expression.Body as MemberExpression;
            MemberInfo mi = member.Member;
            PropertyInfo propInfo = member.Member as PropertyInfo;
            string memberType = member.Expression.Type.Name;
            Type t = member.Expression.Type;
            MemberInfo property = t.GetProperty(propInfo.Name);

            var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
            string displayName = attribute.DisplayName;

            string result = String.Format("<label for=\"{0}\" id=\"{1}\" />{2}</label>", propInfo.Name, propInfo.Name + "_lbl", displayName);
            return new MvcHtmlString(result);
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);
            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();
            if (attribute == null && isRequired)
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            return (T)attribute;
        }

        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");

            var attr = memberInfo.GetAttribute<DisplayNameAttribute>(false);
            if (attr == null)
                return memberInfo.Name;
            return attr.DisplayName;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
                return memberExpr.Member;
            return null;
        }
    }

    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, System.StringComparison.Ordinal);
            if (pos < 0)
                return text;
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}