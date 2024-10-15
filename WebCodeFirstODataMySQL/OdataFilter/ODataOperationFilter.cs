using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

public class ODataOperationFilter : IOperationFilter
{
    private readonly IConfiguration? Configuration;
    public ODataOperationFilter(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        /* var isGetMethod = context.MethodInfo.DeclaringType!
                           .GetMethods()
                           .Any(m => m.Name == context.MethodInfo.Name && m.ReturnType != null && m.CustomAttributes.Any(a => a.AttributeType.Name == "HttpGetAttribute"));

         if (!isGetMethod)
         {


             return;
         }
        */
        #region  for specific method 
        //bool isGetEmployeesMethod = context.MethodInfo.Name == "GetEmployees";

        //if (!isGetEmployeesMethod)
        //{
        //    return;
        //}
        //if(!(context.MethodInfo.Name == "GetEmployees"))
        //{
        //    return ;
        //}
        #endregion

        #region  for more than one method 
        /* var methodsToFilter = new[] { "GetEmployees", "GetAnotherMethod" };
         var isTargetMethod = methodsToFilter.Contains(context.MethodInfo.Name);

         if (!isTargetMethod)
         {
             return;
         }
        */
        #endregion


       

      //  string[] Parameters =  { "GetEmployees", "GetEmployee" };
        var methodQueryOptions = new Dictionary<string, List<string>>();
        var Parameters = Configuration!.GetSection("Parameters").Get<string[]>();
        foreach (var method in Parameters!)
        {

            methodQueryOptions.Add(method, new List<string> { "$filter", "$orderby", "$select", "$top", "$skip", "$expand", "$count" });

        }
            if (!methodQueryOptions.ContainsKey(context.MethodInfo.Name))
            {
                return;
            }

            foreach (var queryOptions in methodQueryOptions.Values)
            {
                foreach (var queryOption in queryOptions)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = queryOption,
                        In = ParameterLocation.Query,
                        Description = $"OData {queryOption.Trim('$')} query",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = (queryOption == "$top" || queryOption == "$skip") ? "int" : "string"
                        }
                    });
                }
            }
        




       

       

       // var queryOptions = methodQueryOptions[context.MethodInfo.Name];


        //if (operation.Parameters == null)
        //{
        //    operation.Parameters = new List<OpenApiParameter>();
        //}



       



        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name = "$filter",
        //    In = ParameterLocation.Query,
        //    Description = "OData filter query",
        //    Required = false,
        //    Schema = new OpenApiSchema
        //    {
        //        Type = "string"
        //    }
        //});


        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name = "$orderby",
        //    In = ParameterLocation.Query,
        //    Description = "OData order by query",
        //    Required = false,
        //    Schema = new OpenApiSchema
        //    {
        //        Type = "string"
        //    }
        //});

        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name = "$select",
        //    In = ParameterLocation.Query,
        //    Description = "OData select query",
        //    Required = false,
        //    Schema = new OpenApiSchema
        //    {
        //        Type = "string"
        //    }
        //});

        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name = "$top",
        //    In = ParameterLocation.Query,
        //    Description = "OData top query",
        //    Required = false,
        //    Schema = new OpenApiSchema
        //    {
        //        Type = "int"
        //    }
        //});

        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name = "$skip",
        //    In = ParameterLocation.Query,
        //    Description = "OData skip query",
        //    Required = false,
        //    Schema = new OpenApiSchema
        //    {
        //        Type = "int"
        //    }
        //});

        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name= "$expand",
        //    In = ParameterLocation.Query,
        //    Description= "OData expand query",
        //    Required = false,
        //    Schema=new OpenApiSchema
        //    {
        //        Type = "string"
        //    }

        //});
        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name = "$count",
        //    In = ParameterLocation.Query,
        //    Description = "Odata count query",
        //    Required = false,
        //    Schema = new OpenApiSchema
        //    {
        //        Type = "long"
        //    }


        //});


    }
}
