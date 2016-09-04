using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ChartControls.TagHelpers
{
    public class ChartTagHelper: TagHelper
    {
        private IList<PropertyInfo> Properties => Source.First().GetType().GetProperties().ToList();

        private IList<string> PropertyNames => Properties.Select(p => p.Name).ToList();
        
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
            
            if (Source == null)
            {
                return;
            }
            
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
                var xKey = PropertyNames.First();
                var yKeys = PropertyNames.Skip(1).ToList();
                
                output.PostElement.AppendHtml($@"
<script>
Morris.{Type}({{
  element: '{Id}',
  data: [
    {ConstructChartData()}
  ],
  xkey: '{xKey}',
  ykeys: [{String.Join(",",yKeys.Select(p=> "'" + p + "'"))}],
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
            var xKey = PropertyNames.First();
            var xProperty = Properties.First();
            var yProperties = Properties.Skip(1);
            
            return String.Join(",", 
                Source.Select(s => $"{{{xKey}:'{xProperty.GetValue(s)}', " + String.Join(",", yProperties.Select(p => p.Name + ":" + p.GetValue(s))) + "}"
            ));
        }
    }
}