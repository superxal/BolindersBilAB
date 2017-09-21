﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.Web.Models;

namespace WU16.BolindersBilAB.Web.Infrastructure
{
    [HtmlTargetElement("div", Attributes ="page-model, page-action, page-query")]
    public class PagingTagHelper : TagHelper
    {
        private IUrlHelperFactory _helper;
        public PagingTagHelper(IUrlHelperFactory Helper)
        {
            _helper = Helper;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public string PageQuery { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(PageModel.CurrentPage * PageModel.ItemsPerPage < PageModel.TotalItems)
            {              
                IUrlHelper urlHelper = _helper.GetUrlHelper(ViewContext);

                var tag = new TagBuilder("a");
                tag.AddCssClass("btn btn-primary");
                tag.Attributes["id"] = "showMore";              
                tag.Attributes["href"] = urlHelper.Action(PageAction, new {page = (PageModel.CurrentPage + 1) });
                tag.InnerHtml.Append("Visa fler");

                output.Content.AppendHtml(tag);
            }
        }
    }
}
