﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Tests.Objects;
using NSubstitute;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace MvcTemplate.Tests.Unit.Components.Mvc
{
    public class TruncatedAttributeTests
    {
        private TruncatedAttribute attribute;
        private ModelBindingContext context;

        public TruncatedAttributeTests()
        {
            attribute = new TruncatedAttribute();
            context = new DefaultModelBindingContext();
            context.ModelState = new ModelStateDictionary();
            context.ValueProvider = Substitute.For<IValueProvider>();
        }

        #region TruncatedAttribute()

        [Fact]
        public void TruncatedAttribute_SetsBinderType()
        {
            Type expected = typeof(TruncatedAttribute);
            Type actual = attribute.BinderType;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region BindModelAsync(ModelBindingContext context)

        [Fact]
        public async Task BindModelAsync_NoValue()
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(DateTime?));
            context.ValueProvider.GetValue(context.ModelName).Returns(ValueProviderResult.None);
            context.ModelMetadata = metadata;

            await attribute.BindModelAsync(context);

            ModelBindingResult expected = new ModelBindingResult();
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task BindModelAsync_TruncatesValue()
        {
            context.ValueProvider.GetValue("TruncatedDateTimeField").Returns(new ValueProviderResult(new DateTime(2017, 2, 3, 4, 5, 6).ToString(), CultureInfo.CurrentCulture));
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForProperty(typeof(AllTypesView), "TruncatedDateTimeField");
            context.ModelName = "TruncatedDateTimeField";
            context.ModelMetadata = metadata;

            await attribute.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success(new DateTime(2017, 2, 3));
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }

        #endregion
    }
}
