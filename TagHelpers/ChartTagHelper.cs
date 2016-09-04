using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ChartControls.TagHelpers
{
    public class ChartTagHelper: TagHelper
    {
        private IEnumerable<PropertyInfo> Properties => Source.First().GetType().GetProperties();
        private PropertyInfo XProperty => Properties.First();
        private IEnumerable<PropertyInfo> YProperties => Properties.Skip(1);
        
        private IList<string> Keys => Properties.Select(p=>p.Name).ToList();
        
        private string XKey => Keys.First();
        
        private IList<string> YKeys => Keys.Skip(1).ToList();
        
        [HtmlAttributeName("id")]
        public string Id { get; set; }
        
        [HtmlAttributeName("type")]
        public ChartType Type { get; set; }
        
        [HtmlAttributeName("labels")]
        public string Labels { get; set; }
        
        [HtmlAttributeName("source")]
        public IEnumerable<object> Source { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("id", Id);
            output.TagName = "div";
            if (Type == ChartType.Donut)
            {
                output.PostElement.AppendHtml($@"
<script>
Morris.{Type}({{
  element: '{Id}',
  data: [
    {ConstructDonutChartData()}
  ]
}});
  </script>");
            }
            else
            {
                 output.PostElement.AppendHtml($@"
<script>
Morris.{Type}({{
  element: '{Id}',
  data: [
    {ConstructChartData()}
  ],
  xkey: '{XKey}',
  ykeys: [{String.Join(",",YKeys.Select(p=> "'" + p + "'"))}],
  labels: [{String.Join(",",Labels.Split(',').Select(l => "'" + l + "'"))}]
}});
  </script>");
            }
        }
        
        private string ConstructDonutChartData()
        {
            var labelProperty = Source.First().GetType().GetProperty("label");
            var valueProperty = Source.First().GetType().GetProperty("value");
            
            return String.Join(",", Source.Select(s => 
                $"{{label: '{labelProperty.GetValue(s)}', value: {valueProperty.GetValue(s)}}}"));
        }
        
        private string ConstructChartData()
        {
            return String.Join(",", 
                Source.Select(s => $"{{{XKey}:'{XProperty.GetValue(s)}', " + String.Join(",", YProperties.Select(p => p.Name + ":" + p.GetValue(s))) + "}"
            ));
        }
    }
}