namespace Kubis1982.Result
{
    using JasperFx.CodeGeneration;
    using JasperFx.CodeGeneration.Frames;
    using JasperFx.CodeGeneration.Model;
    using System;
    using Wolverine.Configuration;
    using Wolverine.Middleware;

    /// <summary>
    /// Implements a continuation strategy for Wolverine that handles <see cref="Result"/> and <see cref="Result{T}"/> return types.
    /// When a handler returns a failed result, it automatically enqueues cascading messages and terminates the handler chain.
    /// </summary>
    public class ResultContinuationStrategy : IContinuationStrategy
    {
        /// <summary>
        /// Attempts to find a continuation handler for the given method call.
        /// Looks for <see cref="Result"/> or <see cref="Result{T}"/> return types and creates appropriate continuation frames.
        /// </summary>
        /// <param name="chain">The handler chain being processed.</param>
        /// <param name="call">The method call to analyze.</param>
        /// <param name="frame">The generated continuation frame if successful; otherwise null.</param>
        /// <returns>True if a continuation handler was found; otherwise false.</returns>
        public bool TryFindContinuationHandler(IChain chain, MethodCall call, out Frame? frame)
        {
            var result = call.Creates.FirstOrDefault(x => x.VariableType == typeof(Result));
            if (result != null)
            {
                frame = new HandlerWithResultFrame(result, GetHandlerReturnType(call));
                return true;
            }

            result = call.Creates.FirstOrDefault(x => x.VariableType.IsGenericType && x.VariableType.GetGenericTypeDefinition() == typeof(Result<>));
            if (result != null)
            {
                frame = new HandlerWithGenericResultFrame(result, GetHandlerReturnType(call));
                return true;
            }

            frame = null;
            return false;
        }

        private static string GetFriendlyTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.FullName ?? type.Name;

            var namePrefix =  (type.GetGenericTypeDefinition().FullName ?? type.Name).Split('`', StringSplitOptions.RemoveEmptyEntries)[0];
            var genericParameters = string.Join(",", type.GetGenericArguments().Select(GetFriendlyTypeName));
            return namePrefix + "<" + genericParameters + ">";
        }
        
        private static Type? GetHandlerReturnType(MethodCall call)
        {
            var returnType = call.HandlerType.GetMethods().FirstOrDefault(m => m.Name.EndsWith("Handle") || m.Name.EndsWith("HandleAsync"))?.ReturnType;

            if (returnType?.GetGenericTypeDefinition() == typeof(Task<>))
                returnType = returnType.GetGenericArguments().FirstOrDefault();
           
            return returnType;
        }

        private static Variable[] GetTupleInnerValues(Variable variable)
        {
            var type = variable.VariableType;

            if (!type.IsGenericType) return [];

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition.FullName?.StartsWith("System.ValueTuple") == true)
            {
                var tupleTypes = type.GetGenericArguments();
                var variables = new Variable[tupleTypes.Length];

                for (var i = 0; i < tupleTypes.Length; i++)
                    variables[i] = new Variable(tupleTypes[i], $"{variable.Usage}.Item{i + 1}");

                return variables;
            }

            if (genericTypeDefinition.FullName?.StartsWith("System.Tuple") == true)
            {
                var tupleTypes = type.GetGenericArguments();
                var variables = new Variable[tupleTypes.Length];

                for (var i = 0; i < tupleTypes.Length; i++)
                    variables[i] = new Variable(tupleTypes[i], $"{variable.Usage}.Item{i + 1}");

                return variables;
            }

            return [];
        }
      
        private class HandlerWithResultFrame : AsyncFrame
        {
            private readonly Type? _handlerReturnType;
            private readonly Variable _result;

            public HandlerWithResultFrame(Variable result, Type? handlerReturnType)
            {
                string resultVariableName = "result_" + Guid.NewGuid().ToString()[0..8];

                result.OverrideName(resultVariableName);

                uses.Add(result);

                _result = result;
                _handlerReturnType = handlerReturnType;
            }

            public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
            {
                writer.BlankLine();
                writer.WriteComment("Result continuation check for Result types");

                writer.Write($"BLOCK:if ({_result.Usage}.IsFailure)");
                if (_handlerReturnType != null)
                    writer.Write(
                        _handlerReturnType != _result.VariableType
                            ? $"await context.EnqueueCascadingAsync(({GetFriendlyTypeName(_handlerReturnType)}){_result.Usage}.Error).ConfigureAwait(false);"
                            : $"await context.EnqueueCascadingAsync({_result.Usage}).ConfigureAwait(false);");
                writer.Write("return;");
                writer.FinishBlock();
                writer.BlankLine();

                Next?.GenerateCode(method, writer);
            }
        }
        
        private class HandlerWithGenericResultFrame : AsyncFrame
        {
            private readonly Type? _handlerReturnType;
            private readonly Variable _result;

            public HandlerWithGenericResultFrame(Variable result, Type? handlerReturnType)
            {
                string resultTypeName = result.VariableType.GetGenericArguments()[0].Name;
                if (resultTypeName.StartsWith("ValueTuple"))
                    resultTypeName = "ValueTuple";
                string resultVariableName = char.ToLower(resultTypeName[0]) + resultTypeName.Substring(1) + "Result_" + Guid.NewGuid().ToString()[0..8];

                result.OverrideName(resultVariableName);

                uses.Add(
                    new Variable(result.VariableType.GetGenericArguments()[0],
                    result.Usage + "_SuccessValue"));

                creates.Add(
                    new Variable(result.VariableType.GetGenericArguments()[0], 
                    result.Usage + "_SuccessValue"));

                var tupleInnerValues = GetTupleInnerValues(
                    new Variable(result.VariableType.GetGenericArguments()[0],
                    result.Usage + "_SuccessValue"));

                foreach (var innerValue in tupleInnerValues) creates.Add(innerValue);
                
                _result = result;
                _handlerReturnType = handlerReturnType;
            }

            public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
            {
                writer.BlankLine();
                writer.WriteComment("Result continuation check for Result<T> types");

                writer.Write($"BLOCK:if ({_result.Usage}.IsFailure)");
                if (_handlerReturnType != null)
                    writer.Write(
                        _handlerReturnType != _result.VariableType
                            ? $"await context.EnqueueCascadingAsync(({GetFriendlyTypeName(_handlerReturnType)}){_result.Usage}.Error).ConfigureAwait(false);"
                            : $"await context.EnqueueCascadingAsync({_result.Usage}).ConfigureAwait(false);");
                writer.Write("return;");
                writer.FinishBlock();

                writer.WriteComment("Extracting the success value from Result<T>");
                writer.WriteLine($"var {_result.Usage}_SuccessValue = {_result.Usage}.Value;");

                Next?.GenerateCode(method, writer);
            }
        }
    }
}
