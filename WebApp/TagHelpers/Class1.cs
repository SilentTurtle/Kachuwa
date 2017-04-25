﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.TagHelpers
{
    public class Class1
    {
    }
    /// <summary>
    /// Default implementation for <see cref="IViewComponentHelper"/>.
    /// </summary>
    public class Default2ViewComponentHelper : IViewComponentHelper, IViewContextAware
    {
        private readonly IViewComponentDescriptorCollectionProvider _descriptorProvider;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IViewComponentInvokerFactory _invokerFactory;
        private readonly IViewComponentSelector _selector;
        private readonly IViewBufferScope _viewBufferScope;
        private ViewContext _viewContext;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultViewComponentHelper"/>.
        /// </summary>
        /// <param name="descriptorProvider">The <see cref="IViewComponentDescriptorCollectionProvider"/>
        /// used to locate view components.</param>
        /// <param name="htmlEncoder">The <see cref="HtmlEncoder"/>.</param>
        /// <param name="selector">The <see cref="IViewComponentSelector"/>.</param>
        /// <param name="invokerFactory">The <see cref="IViewComponentInvokerFactory"/>.</param>
        /// <param name="viewBufferScope">The <see cref="IViewBufferScope"/> that manages the lifetime of
        /// <see cref="ViewBuffer"/> instances.</param>
        public Default2ViewComponentHelper(
            IViewComponentDescriptorCollectionProvider descriptorProvider,
            HtmlEncoder htmlEncoder,
            IViewComponentSelector selector,
            IViewComponentInvokerFactory invokerFactory,
            IViewBufferScope viewBufferScope)
        {
            if (descriptorProvider == null)
            {
                throw new ArgumentNullException(nameof(descriptorProvider));
            }

            if (htmlEncoder == null)
            {
                throw new ArgumentNullException(nameof(htmlEncoder));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (invokerFactory == null)
            {
                throw new ArgumentNullException(nameof(invokerFactory));
            }

            if (viewBufferScope == null)
            {
                throw new ArgumentNullException(nameof(viewBufferScope));
            }

            _descriptorProvider = descriptorProvider;
            _htmlEncoder = htmlEncoder;
            _selector = selector;
            _invokerFactory = invokerFactory;
            _viewBufferScope = viewBufferScope;
        }

        /// <inheritdoc />
        public void Contextualize(ViewContext viewContext)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            _viewContext = viewContext;
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(string name, object arguments)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var descriptor = _selector.SelectComponent(name);
            if (descriptor == null)
            {
                throw new InvalidOperationException("asdfasd");
            }

            return InvokeCoreAsync(descriptor, arguments);
        }

        /// <inheritdoc />
        public Task<IHtmlContent> InvokeAsync(Type componentType, object arguments)
        {
            if (componentType == null)
            {
                throw new ArgumentNullException(nameof(componentType));
            }

            var descriptor = SelectComponent(componentType);
            return InvokeCoreAsync(descriptor, arguments);
        }

        private ViewComponentDescriptor SelectComponent(Type componentType)
        {
            var descriptors = _descriptorProvider.ViewComponents;
            for (var i = 0; i < descriptors.Items.Count; i++)
            {
                var descriptor = descriptors.Items[i];
                if (descriptor.TypeInfo == componentType?.GetTypeInfo())
                {
                    return descriptor;
                }
            }

            throw new InvalidOperationException("Resources.FormatViewComponent_CannotFindComponent(componentType.FullName)");
        }

        // Internal for testing
        internal IDictionary<string, object> GetArgumentDictionary(ViewComponentDescriptor descriptor, object arguments)
        {
            if (arguments != null)
            {
                if (descriptor.Parameters.Count == 1 && descriptor.Parameters[0].ParameterType.IsAssignableFrom(arguments.GetType()))
                {
                    return new Dictionary<string, object>(capacity: 1, comparer: StringComparer.OrdinalIgnoreCase)
                    {
                        { descriptor.Parameters[0].Name, arguments }
                    };
                }
            }

            return PropertyHelper.ObjectToDictionary(arguments);
        }

        private async Task<IHtmlContent> InvokeCoreAsync(ViewComponentDescriptor descriptor, object arguments)
        {
            var argumentDictionary = GetArgumentDictionary(descriptor, arguments);

            var viewBuffer = new ViewBuffer(_viewBufferScope, descriptor.FullName, ViewBuffer.ViewComponentPageSize);
            using (var writer = new ViewBufferTextWriter(viewBuffer, _viewContext.Writer.Encoding))
            {
                var context = new ViewComponentContext(descriptor, argumentDictionary, _htmlEncoder, _viewContext, writer);

                var invoker = _invokerFactory.CreateInstance(context);
                if (invoker == null)
                {
                    throw new InvalidOperationException(
                       " Resources.FormatViewComponent_IViewComponentFactory_ReturnedNull(descriptor.FullName)");
                }

                await invoker.InvokeAsync(context);
                return viewBuffer;
            }
        }
    }
    internal class PropertyHelper
    {
        // Delegate type for a by-ref property getter
        private delegate TValue ByRefFunc<TDeclaringType, TValue>(ref TDeclaringType arg);

        private static readonly MethodInfo CallPropertyGetterOpenGenericMethod =
            typeof(PropertyHelper).GetTypeInfo().GetDeclaredMethod(nameof(CallPropertyGetter));

        private static readonly MethodInfo CallPropertyGetterByReferenceOpenGenericMethod =
            typeof(PropertyHelper).GetTypeInfo().GetDeclaredMethod(nameof(CallPropertyGetterByReference));

        private static readonly MethodInfo CallNullSafePropertyGetterOpenGenericMethod =
            typeof(PropertyHelper).GetTypeInfo().GetDeclaredMethod(nameof(CallNullSafePropertyGetter));

        private static readonly MethodInfo CallNullSafePropertyGetterByReferenceOpenGenericMethod =
            typeof(PropertyHelper).GetTypeInfo().GetDeclaredMethod(nameof(CallNullSafePropertyGetterByReference));

        private static readonly MethodInfo CallPropertySetterOpenGenericMethod =
            typeof(PropertyHelper).GetTypeInfo().GetDeclaredMethod(nameof(CallPropertySetter));

        // Using an array rather than IEnumerable, as target will be called on the hot path numerous times.
        private static readonly ConcurrentDictionary<Type, PropertyHelper[]> PropertiesCache =
            new ConcurrentDictionary<Type, PropertyHelper[]>();

        private static readonly ConcurrentDictionary<Type, PropertyHelper[]> VisiblePropertiesCache =
            new ConcurrentDictionary<Type, PropertyHelper[]>();

        private Action<object, object> _valueSetter;
        private Func<object, object> _valueGetter;

        /// <summary>
        /// Initializes a fast <see cref="PropertyHelper"/>.
        /// This constructor does not cache the helper. For caching, use <see cref="GetProperties(object)"/>.
        /// </summary>
        public PropertyHelper(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            Property = property;
            Name = property.Name;
        }

        /// <summary>
        /// Gets the backing <see cref="PropertyInfo"/>.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets (or sets in derived types) the property name.
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Gets the property value getter.
        /// </summary>
        public Func<object, object> ValueGetter
        {
            get
            {
                if (_valueGetter == null)
                {
                    _valueGetter = MakeFastPropertyGetter(Property);
                }

                return _valueGetter;
            }
        }

        /// <summary>
        /// Gets the property value setter.
        /// </summary>
        public Action<object, object> ValueSetter
        {
            get
            {
                if (_valueSetter == null)
                {
                    _valueSetter = MakeFastPropertySetter(Property);
                }

                return _valueSetter;
            }
        }

        /// <summary>
        /// Returns the property value for the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The object whose property value will be returned.</param>
        /// <returns>The property value.</returns>
        public object GetValue(object instance)
        {
            return ValueGetter(instance);
        }

        /// <summary>
        /// Sets the property value for the specified <paramref name="instance" />.
        /// </summary>
        /// <param name="instance">The object whose property value will be set.</param>
        /// <param name="value">The property value.</param>
        public void SetValue(object instance, object value)
        {
            ValueSetter(instance, value);
        }

        /// <summary>
        /// Creates and caches fast property helpers that expose getters for every public get property on the
        /// underlying type.
        /// </summary>
        /// <param name="instance">the instance to extract property accessors for.</param>
        /// <returns>a cached array of all public property getters from the underlying type of target instance.
        /// </returns>
        public static PropertyHelper[] GetProperties(object instance)
        {
            return GetProperties(instance.GetType());
        }

        /// <summary>
        /// Creates and caches fast property helpers that expose getters for every public get property on the
        /// specified type.
        /// </summary>
        /// <param name="type">the type to extract property accessors for.</param>
        /// <returns>a cached array of all public property getters from the type of target instance.
        /// </returns>
        public static PropertyHelper[] GetProperties(Type type)
        {
            return GetProperties(type, CreateInstance, PropertiesCache);
        }

        /// <summary>
        /// <para>
        /// Creates and caches fast property helpers that expose getters for every non-hidden get property
        /// on the specified type.
        /// </para>
        /// <para>
        /// <see cref="M:GetVisibleProperties"/> excludes properties defined on base types that have been
        /// hidden by definitions using the <c>new</c> keyword.
        /// </para>
        /// </summary>
        /// <param name="instance">The instance to extract property accessors for.</param>
        /// <returns>
        /// A cached array of all public property getters from the instance's type.
        /// </returns>
        public static PropertyHelper[] GetVisibleProperties(object instance)
        {
            return GetVisibleProperties(instance.GetType(), CreateInstance, PropertiesCache, VisiblePropertiesCache);
        }

        /// <summary>
        /// <para>
        /// Creates and caches fast property helpers that expose getters for every non-hidden get property
        /// on the specified type.
        /// </para>
        /// <para>
        /// <see cref="M:GetVisibleProperties"/> excludes properties defined on base types that have been
        /// hidden by definitions using the <c>new</c> keyword.
        /// </para>
        /// </summary>
        /// <param name="type">The type to extract property accessors for.</param>
        /// <returns>
        /// A cached array of all public property getters from the type.
        /// </returns>
        public static PropertyHelper[] GetVisibleProperties(Type type)
        {
            return GetVisibleProperties(type, CreateInstance, PropertiesCache, VisiblePropertiesCache);
        }

        /// <summary>
        /// Creates a single fast property getter. The result is not cached.
        /// </summary>
        /// <param name="propertyInfo">propertyInfo to extract the getter for.</param>
        /// <returns>a fast getter.</returns>
        /// <remarks>
        /// This method is more memory efficient than a dynamically compiled lambda, and about the
        /// same speed.
        /// </remarks>
        public static Func<object, object> MakeFastPropertyGetter(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo != null);

            return MakeFastPropertyGetter(
                propertyInfo,
                CallPropertyGetterOpenGenericMethod,
                CallPropertyGetterByReferenceOpenGenericMethod);
        }

        /// <summary>
        /// Creates a single fast property getter which is safe for a null input object. The result is not cached.
        /// </summary>
        /// <param name="propertyInfo">propertyInfo to extract the getter for.</param>
        /// <returns>a fast getter.</returns>
        /// <remarks>
        /// This method is more memory efficient than a dynamically compiled lambda, and about the
        /// same speed.
        /// </remarks>
        public static Func<object, object> MakeNullSafeFastPropertyGetter(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo != null);

            return MakeFastPropertyGetter(
                propertyInfo,
                CallNullSafePropertyGetterOpenGenericMethod,
                CallNullSafePropertyGetterByReferenceOpenGenericMethod);
        }

        private static Func<object, object> MakeFastPropertyGetter(
            PropertyInfo propertyInfo,
            MethodInfo propertyGetterWrapperMethod,
            MethodInfo propertyGetterByRefWrapperMethod)
        {
            Debug.Assert(propertyInfo != null);

            // Must be a generic method with a Func<,> parameter
            Debug.Assert(propertyGetterWrapperMethod != null);
            Debug.Assert(propertyGetterWrapperMethod.IsGenericMethodDefinition);
            Debug.Assert(propertyGetterWrapperMethod.GetParameters().Length == 2);

            // Must be a generic method with a ByRefFunc<,> parameter
            Debug.Assert(propertyGetterByRefWrapperMethod != null);
            Debug.Assert(propertyGetterByRefWrapperMethod.IsGenericMethodDefinition);
            Debug.Assert(propertyGetterByRefWrapperMethod.GetParameters().Length == 2);

            var getMethod = propertyInfo.GetMethod;
            Debug.Assert(getMethod != null);
            Debug.Assert(!getMethod.IsStatic);
            Debug.Assert(getMethod.GetParameters().Length == 0);

            // Instance methods in the CLR can be turned into static methods where the first parameter
            // is open over "target". This parameter is always passed by reference, so we have a code
            // path for value types and a code path for reference types.
            if (getMethod.DeclaringType.GetTypeInfo().IsValueType)
            {
                // Create a delegate (ref TDeclaringType) -> TValue
                return MakeFastPropertyGetter(
                    typeof(ByRefFunc<,>),
                    getMethod,
                    propertyGetterByRefWrapperMethod);
            }
            else
            {
                // Create a delegate TDeclaringType -> TValue
                return MakeFastPropertyGetter(
                    typeof(Func<,>),
                    getMethod,
                    propertyGetterWrapperMethod);
            }
        }

        private static Func<object, object> MakeFastPropertyGetter(
            Type openGenericDelegateType,
            MethodInfo propertyGetMethod,
            MethodInfo openGenericWrapperMethod)
        {
            var typeInput = propertyGetMethod.DeclaringType;
            var typeOutput = propertyGetMethod.ReturnType;

            var delegateType = openGenericDelegateType.MakeGenericType(typeInput, typeOutput);
            var propertyGetterDelegate = propertyGetMethod.CreateDelegate(delegateType);

            var wrapperDelegateMethod = openGenericWrapperMethod.MakeGenericMethod(typeInput, typeOutput);
            var accessorDelegate = wrapperDelegateMethod.CreateDelegate(
                typeof(Func<object, object>),
                propertyGetterDelegate);

            return (Func<object, object>)accessorDelegate;
        }

        /// <summary>
        /// Creates a single fast property setter for reference types. The result is not cached.
        /// </summary>
        /// <param name="propertyInfo">propertyInfo to extract the setter for.</param>
        /// <returns>a fast getter.</returns>
        /// <remarks>
        /// This method is more memory efficient than a dynamically compiled lambda, and about the
        /// same speed. This only works for reference types.
        /// </remarks>
        public static Action<object, object> MakeFastPropertySetter(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo != null);
            Debug.Assert(!propertyInfo.DeclaringType.GetTypeInfo().IsValueType);

            var setMethod = propertyInfo.SetMethod;
            Debug.Assert(setMethod != null);
            Debug.Assert(!setMethod.IsStatic);
            Debug.Assert(setMethod.ReturnType == typeof(void));
            var parameters = setMethod.GetParameters();
            Debug.Assert(parameters.Length == 1);

            // Instance methods in the CLR can be turned into static methods where the first parameter
            // is open over "target". This parameter is always passed by reference, so we have a code
            // path for value types and a code path for reference types.
            var typeInput = setMethod.DeclaringType;
            var parameterType = parameters[0].ParameterType;

            // Create a delegate TDeclaringType -> { TDeclaringType.Property = TValue; }
            var propertySetterAsAction =
                setMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(typeInput, parameterType));
            var callPropertySetterClosedGenericMethod =
                CallPropertySetterOpenGenericMethod.MakeGenericMethod(typeInput, parameterType);
            var callPropertySetterDelegate =
                callPropertySetterClosedGenericMethod.CreateDelegate(
                    typeof(Action<object, object>), propertySetterAsAction);

            return (Action<object, object>)callPropertySetterDelegate;
        }

        /// <summary>
        /// Given an object, adds each instance property with a public get method as a key and its
        /// associated value to a dictionary.
        ///
        /// If the object is already an <see cref="IDictionary{String, Object}"/> instance, then a copy
        /// is returned.
        /// </summary>
        /// <remarks>
        /// The implementation of PropertyHelper will cache the property accessors per-type. This is
        /// faster when the the same type is used multiple times with ObjectToDictionary.
        /// </remarks>
        public static IDictionary<string, object> ObjectToDictionary(object value)
        {
            var dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                return new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
            }

            dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (value != null)
            {
                foreach (var helper in GetProperties(value))
                {
                    dictionary[helper.Name] = helper.GetValue(value);
                }
            }

            return dictionary;
        }

        private static PropertyHelper CreateInstance(PropertyInfo property)
        {
            return new PropertyHelper(property);
        }

        // Called via reflection
        private static object CallPropertyGetter<TDeclaringType, TValue>(
            Func<TDeclaringType, TValue> getter,
            object target)
        {
            return getter((TDeclaringType)target);
        }

        // Called via reflection
        private static object CallPropertyGetterByReference<TDeclaringType, TValue>(
            ByRefFunc<TDeclaringType, TValue> getter,
            object target)
        {
            var unboxed = (TDeclaringType)target;
            return getter(ref unboxed);
        }

        // Called via reflection
        private static object CallNullSafePropertyGetter<TDeclaringType, TValue>(
            Func<TDeclaringType, TValue> getter,
            object target)
        {
            if (target == null)
            {
                return null;
            }

            return getter((TDeclaringType)target);
        }

        // Called via reflection
        private static object CallNullSafePropertyGetterByReference<TDeclaringType, TValue>(
            ByRefFunc<TDeclaringType, TValue> getter,
            object target)
        {
            if (target == null)
            {
                return null;
            }

            var unboxed = (TDeclaringType)target;
            return getter(ref unboxed);
        }

        private static void CallPropertySetter<TDeclaringType, TValue>(
            Action<TDeclaringType, TValue> setter,
            object target,
            object value)
        {
            setter((TDeclaringType)target, (TValue)value);
        }

        protected static PropertyHelper[] GetVisibleProperties(
            Type type,
            Func<PropertyInfo, PropertyHelper> createPropertyHelper,
            ConcurrentDictionary<Type, PropertyHelper[]> allPropertiesCache,
            ConcurrentDictionary<Type, PropertyHelper[]> visiblePropertiesCache)
        {
            PropertyHelper[] result;
            if (visiblePropertiesCache.TryGetValue(type, out result))
            {
                return result;
            }

            // The simple and common case, this is normal POCO object - no need to allocate.
            var allPropertiesDefinedOnType = true;
            var allProperties = GetProperties(type, createPropertyHelper, allPropertiesCache);
            foreach (var propertyHelper in allProperties)
            {
                if (propertyHelper.Property.DeclaringType != type)
                {
                    allPropertiesDefinedOnType = false;
                    break;
                }
            }

            if (allPropertiesDefinedOnType)
            {
                result = allProperties;
                visiblePropertiesCache.TryAdd(type, result);
                return result;
            }

            // There's some inherited properties here, so we need to check for hiding via 'new'.
            var filteredProperties = new List<PropertyHelper>(allProperties.Length);
            foreach (var propertyHelper in allProperties)
            {
                var declaringType = propertyHelper.Property.DeclaringType;
                if (declaringType == type)
                {
                    filteredProperties.Add(propertyHelper);
                    continue;
                }

                // If this property was declared on a base type then look for the definition closest to the
                // the type to see if we should include it.
                var ignoreProperty = false;

                // Walk up the hierarchy until we find the type that actally declares this
                // PropertyInfo.
                var currentTypeInfo = type.GetTypeInfo();
                var declaringTypeInfo = declaringType.GetTypeInfo();
                while (currentTypeInfo != null && currentTypeInfo != declaringTypeInfo)
                {
                    // We've found a 'more proximal' public definition
                    var declaredProperty = currentTypeInfo.GetDeclaredProperty(propertyHelper.Name);
                    if (declaredProperty != null)
                    {
                        ignoreProperty = true;
                        break;
                    }

                    currentTypeInfo = currentTypeInfo.BaseType?.GetTypeInfo();
                }

                if (!ignoreProperty)
                {
                    filteredProperties.Add(propertyHelper);
                }
            }

            result = filteredProperties.ToArray();
            visiblePropertiesCache.TryAdd(type, result);
            return result;
        }

        protected static PropertyHelper[] GetProperties(
            Type type,
            Func<PropertyInfo, PropertyHelper> createPropertyHelper,
            ConcurrentDictionary<Type, PropertyHelper[]> cache)
        {
            // Unwrap nullable types. This means Nullable<T>.Value and Nullable<T>.HasValue will not be
            // part of the sequence of properties returned by this method.
            type = Nullable.GetUnderlyingType(type) ?? type;

            PropertyHelper[] helpers;
            if (!cache.TryGetValue(type, out helpers))
            {
                // We avoid loading indexed properties using the Where statement.
                var properties = type.GetRuntimeProperties().Where(IsInterestingProperty);

                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsInterface)
                {
                    // Reflection does not return information about inherited properties on the interface itself.
                    properties = properties.Concat(typeInfo.ImplementedInterfaces.SelectMany(
                        interfaceType => interfaceType.GetRuntimeProperties().Where(IsInterestingProperty)));
                }

                helpers = properties.Select(p => createPropertyHelper(p)).ToArray();
                cache.TryAdd(type, helpers);
            }

            return helpers;
        }

        // Indexed properties are not useful (or valid) for grabbing properties off an object.
        private static bool IsInterestingProperty(PropertyInfo property)
        {
            // For imporving application startup time, do not use GetIndexParameters() api early in this check as it
            // creates a copy of parameter array and also we would like to check for the presence of a get method
            // and short circuit asap.
            return property.GetMethod != null &&
                property.GetMethod.IsPublic &&
                !property.GetMethod.IsStatic &&
                property.GetMethod.GetParameters().Length == 0;
        }
    }

    public class Default2ViewComponentSelector : IViewComponentSelector
    {
        private readonly IViewComponentDescriptorCollectionProvider _descriptorProvider;

        private ViewComponentDescriptorCache _cache;

        /// <summary>
        /// Creates a new <see cref="DefaultViewComponentSelector"/>.
        /// </summary>
        /// <param name="descriptorProvider">The <see cref="IViewComponentDescriptorCollectionProvider"/>.</param>
        public Default2ViewComponentSelector(IViewComponentDescriptorCollectionProvider descriptorProvider)
        {
            _descriptorProvider = descriptorProvider;
        }

        /// <inheritdoc />
        public ViewComponentDescriptor SelectComponent(string componentName)
        {
            if (componentName == null)
            {
                throw new ArgumentNullException(nameof(componentName));
            }

            var collection = _descriptorProvider.ViewComponents;
            if (_cache == null || _cache.Version != collection.Version)
            {
                _cache = new ViewComponentDescriptorCache(collection);
            }

            // ViewComponent names can either be fully-qualified, or refer to the 'short-name'. If the provided
            // name contains a '.' - then it's a fully-qualified name.
            if (componentName.Contains("."))
            {
                return _cache.SelectByFullName(componentName);
            }
            else
            {
                return _cache.SelectByShortName(componentName);
            }
        }

        private class ViewComponentDescriptorCache
        {
            private readonly ILookup<string, ViewComponentDescriptor> _lookupByShortName;
            private readonly ILookup<string, ViewComponentDescriptor> _lookupByFullName;

            public ViewComponentDescriptorCache(ViewComponentDescriptorCollection collection)
            {
                Version = collection.Version;

                _lookupByShortName = collection.Items.ToLookup(c => c.ShortName, c => c);
                _lookupByFullName = collection.Items.ToLookup(c => c.FullName, c => c);
            }

            public int Version { get; }

            public ViewComponentDescriptor SelectByShortName(string name)
            {
                return Select(_lookupByShortName, name);
            }

            public ViewComponentDescriptor SelectByFullName(string name)
            {
                return Select(_lookupByFullName, name);
            }

            private static ViewComponentDescriptor Select(
                ILookup<string, ViewComponentDescriptor> candidates,
                string name)
            {
                var matches = candidates[name];

                var count = matches.Count();
                if (count == 0)
                {
                    return null;
                }
                else if (count == 1)
                {
                    return matches.Single();
                }
                else
                {
                    var matchedTypes = new List<string>();
                    foreach (var candidate in matches)
                    {
                        matchedTypes.Add("Resources.FormatViewComponent_AmbiguousTypeMatch_Item(candidate.TypeInfo.FullName,candidate.FullName)");
                    }

                    var typeNames = string.Join(Environment.NewLine, matchedTypes);
                    throw new InvalidOperationException(
                        "Resources.FormatViewComponent_AmbiguousTypeMatch(name, Environment.NewLine, typeNames)");
                }
            }
        }
    }

   
}