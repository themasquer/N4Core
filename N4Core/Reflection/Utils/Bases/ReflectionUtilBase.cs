#nullable disable

using N4Core.Files.Bases;
using N4Core.Records.Bases;
using N4Core.Reflection.Attributes;
using N4Core.Reflection.Models;
using N4Core.Types.Extensions;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace N4Core.Reflection.Utils.Bases
{
    public abstract class ReflectionUtilBase
    {
        public virtual PropertyInfo GetPropertyInfo<T>(T instance, string propertyName) where T : class, new()
        {
            return instance.GetType().GetProperty(propertyName);
        }

        public virtual PropertyInfo GetPropertyInfo<T>(string propertyName) where T : class, new()
        {
            return typeof(T).GetProperty(propertyName);
        }

        public virtual List<ReflectionPropertyModel> GetReflectionPropertyModelProperties<T>(TagAttributes tagAttribute = TagAttributes.None) where T : class
        {
            List<ReflectionPropertyModel> reflectionPropertyModelProperties = null;
            PropertyInfo[] propertyInfoArray = typeof(T).GetProperties();
            List<Attribute> customAttributes;
            Type attributeType;
            string displayName;
            bool attributeFound;
            if (propertyInfoArray is not null && propertyInfoArray.Length > 0)
            {
                reflectionPropertyModelProperties = new List<ReflectionPropertyModel>();
                if (tagAttribute == TagAttributes.None)
                {
                    foreach (var propertyInfo in propertyInfoArray)
                    {
                        displayName = string.Empty;
                        customAttributes = propertyInfo.GetCustomAttributes().ToList();
                        if (customAttributes is not null && customAttributes.Count > 0)
                        {
                            foreach (var customAttribute in customAttributes)
                            {
                                if (customAttribute.GetType() == typeof(DisplayNameAttribute))
                                {
                                    displayName = ((DisplayNameAttribute)customAttribute).DisplayName;
                                    break;
                                }
                            }
                        }
                        reflectionPropertyModelProperties.Add(new ReflectionPropertyModel(propertyInfo.Name, displayName));
                    }
                }
                else if (tagAttribute == TagAttributes.Order || tagAttribute == TagAttributes.StringFilter || tagAttribute == TagAttributes.Export)
                {
                    switch (tagAttribute)
                    {
                        case TagAttributes.Order:
                            attributeType = typeof(OrderTagAttribute);
                            break;
                        case TagAttributes.StringFilter:
                            attributeType = typeof(StringFilterTagAttribute);
                            break;
                        default:
                            attributeType = typeof(ExportTagAttribute);
                            break;
                    }
                    foreach (var propertyInfo in propertyInfoArray)
                    {
                        displayName = string.Empty;
                        attributeFound = false;
                        customAttributes = propertyInfo.GetCustomAttributes().ToList();
                        if (customAttributes is not null && customAttributes.Count > 0)
                        {
                            foreach (var customAttribute in customAttributes)
                            {
                                if (customAttribute.GetType() == attributeType)
                                    attributeFound = true;
                                if (customAttribute.GetType() == typeof(DisplayNameAttribute))
                                    displayName = ((DisplayNameAttribute)customAttribute).DisplayName;
                            }
                            if (attributeFound)
                                reflectionPropertyModelProperties.Add(new ReflectionPropertyModel(propertyInfo.Name, displayName));
                        }
                    }
                }
            }
            return reflectionPropertyModelProperties;
        }

        public virtual void TrimStringProperties<T>(T instance) where T : class, new()
        {
            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)).ToList();
            string value;
            if (properties is not null && properties.Count > 0)
            {
                foreach (var property in properties)
                {
                    value = (string)property.GetValue(instance);
                    property.SetValue(instance, value?.Trim());
                }
            }
        }

        public virtual Expression<Func<T, bool>> GetPredicateContainsExpression<T>(string propertyName, string value, bool isCaseInsensitive = true) where T : class, new()
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            var property = Expression.Property(parameter, propertyName);
            var containsMethod = typeof(string).GetMethods().Where(m => m.Name == "Contains" && m.GetParameters()[0].ParameterType == typeof(string)).FirstOrDefault();
            MethodCallExpression containsCall;
            if (isCaseInsensitive)
            {
                var toUpperMethod = typeof(string).GetMethods().Where(m => m.Name == "ToUpper").FirstOrDefault();
                containsCall = Expression.Call(Expression.Call(property, toUpperMethod), containsMethod, Expression.Constant(value ?? string.Empty));
            }
            else
            {
                containsCall = Expression.Call(property, containsMethod, Expression.Constant(value ?? string.Empty));
            }
            return Expression.Lambda<Func<T, bool>>(containsCall, parameter);
        }

        public virtual Expression<Func<T, object>> GetExpression<T>(string propertyName) where T : class, new()
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            Expression conversion = Expression.Convert(Expression.Property(parameter, propertyName), typeof(object));
            return Expression.Lambda<Func<T, object>>(conversion, parameter);
        }

        public virtual IQueryable<T> OrderQuery<T>(IQueryable<T> query, bool orderDirectionDescending, string orderExpression, string orderExpressionSuffix) where T : class, new()
        {
            if (orderExpression is null)
                return query;
            orderExpression = orderExpression.FirstLetterToUpperOthersToLower();
            var property = GetPropertyInfo<T>(orderExpression);
            if (property is null)
                return query;
            var valueProperty = orderExpression.EndsWith(orderExpressionSuffix) ? GetPropertyInfo<T>(orderExpression.Substring(0, orderExpression.Length - orderExpressionSuffix.Length)) : GetPropertyInfo<T>(orderExpression);
            if (valueProperty is null)
                valueProperty = GetPropertyInfo<T>(orderExpression);
            if (valueProperty is null)
                return query;
            ParameterExpression parameter = Expression.Parameter(typeof(T), "c");
            Expression body = valueProperty.Name.Split('.').Aggregate<string, Expression>(parameter, Expression.PropertyOrField);
            return orderDirectionDescending
                ? (IOrderedQueryable<T>)Queryable.OrderByDescending(query, (dynamic)Expression.Lambda(body, parameter))
                : (IOrderedQueryable<T>)Queryable.OrderBy(query, (dynamic)Expression.Lambda(body, parameter));
        }

        public virtual DataTable ConvertToDataTable<T>(List<T> list) where T : class, new()
        {
            DataTable dataTable = null;
            PropertyInfo propertyInfo;
            object propertyValue;
            var properties = GetReflectionPropertyModelProperties<T>(TagAttributes.Export);
            var displayNameAttributes = properties.Select(p => p.DisplayName).ToList();
            if (properties is not null && displayNameAttributes is not null && displayNameAttributes.Count > 0)
            {
                dataTable = new DataTable();
                for (int i = 0; i < properties.Count; i++)
                {
                    propertyInfo = GetPropertyInfo<T>(properties[i].Name);
                    if (propertyInfo is not null)
                        dataTable.Columns.Add(displayNameAttributes[i], Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }
                foreach (T item in list)
                {
                    DataRow row = dataTable.NewRow();
                    for (int i = 0; i < properties.Count; i++)
                    {
                        propertyValue = GetPropertyInfo(item, properties[i].Name).GetValue(item);
                        row[displayNameAttributes[i]] = propertyValue ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }

        public virtual ReflectionRecordModel GetReflectionRecordModel<T>() where T : class, new()
        {
            var reflectionRecordModel = new ReflectionRecordModel();
            var property = GetReflectionPropertyModelProperties<ISoftDelete>()?[0];
            reflectionRecordModel.IsDeleted = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = GetReflectionPropertyModelProperties<IModifiedBy>()?[0];
            reflectionRecordModel.CreateDate = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = GetReflectionPropertyModelProperties<IModifiedBy>()?[1];
            reflectionRecordModel.CreatedBy = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = GetReflectionPropertyModelProperties<IModifiedBy>()?[2];
            reflectionRecordModel.UpdateDate = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = GetReflectionPropertyModelProperties<IModifiedBy>()?[3];
            reflectionRecordModel.UpdatedBy = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            reflectionRecordModel.Guid = GetReflectionPropertyModelProperties<Record>()?[1]?.Name;
            property = GetReflectionPropertyModelProperties<RecordFile>()?[0];
            reflectionRecordModel.FileData = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = GetReflectionPropertyModelProperties<RecordFile>()?[1];
            reflectionRecordModel.FileContent = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            property = GetReflectionPropertyModelProperties<RecordFile>()?[2];
            reflectionRecordModel.FilePath = GetReflectionPropertyModelProperties<T>()?.FirstOrDefault(pm => pm.Name == property?.Name)?.Name;
            return reflectionRecordModel;
        }
    }
}
