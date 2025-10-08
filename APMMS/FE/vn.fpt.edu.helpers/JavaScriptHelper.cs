using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FE.vn.fpt.edu.helpers
{
    public static class JavaScriptHelper
    {
        public static IHtmlContent RenderApiCall(string endpoint, string method = "GET", object? data = null)
        {
            var script = $@"
                <script>
                    async function callApi() {{
                        try {{
                            const response = await fetch('/api/{endpoint}', {{
                                method: '{method}',
                                headers: {{
                                    'Content-Type': 'application/json',
                                }},
                                {(data != null ? $"body: JSON.stringify({data})," : "")}
                            }});
                            
                            if (!response.ok) {{
                                throw new Error('Network response was not ok');
                            }}
                            
                            const result = await response.json();
                            console.log('API Response:', result);
                            return result;
                        }} catch (error) {{
                            console.error('API Error:', error);
                            throw error;
                        }}
                    }}
                </script>
            ";

            return new HtmlString(script);
        }

        public static IHtmlContent RenderValidationScript()
        {
            var script = @"
                <script>
                    function validateForm(formId) {
                        const form = document.getElementById(formId);
                        if (!form) return false;
                        
                        const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
                        let isValid = true;
                        
                        inputs.forEach(input => {
                            if (!input.value.trim()) {
                                input.classList.add('is-invalid');
                                isValid = false;
                            } else {
                                input.classList.remove('is-invalid');
                            }
                        });
                        
                        return isValid;
                    }
                </script>
            ";

            return new HtmlString(script);
        }
    }
}
