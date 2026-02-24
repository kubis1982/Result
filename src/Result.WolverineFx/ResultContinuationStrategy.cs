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
                frame = new MaybeEndHandlerWithResultFrame(result, GetHandlerReturnType(call));
                return true;
            }

            result = call.Creates.FirstOrDefault(x => x.VariableType.IsGenericType && x.VariableType.GetGenericTypeDefinition() == typeof(Result<>));
            if (result != null)
            {
                frame = new MaybeEndHandlerWithGenericResultFrame(result, GetHandlerReturnType(call));
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

            // Check if the type is a tuple (ValueTuple)
            if (!type.IsGenericType)
                return [];

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            // Check for ValueTuple types (ValueTuple<T1>, ValueTuple<T1,T2>, etc.)
            if (genericTypeDefinition.FullName?.StartsWith("System.ValueTuple") == true)
            {
                var tupleTypes = type.GetGenericArguments();
                var variables = new Variable[tupleTypes.Length];

                for (var i = 0; i < tupleTypes.Length; i++)
                    variables[i] = new Variable(tupleTypes[i], $"{variable.Usage}.Item{i + 1}");

                return variables;
            }

            // Check for legacy Tuple types (Tuple<T1>, Tuple<T1,T2>, etc.)
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

      
        private class MaybeEndHandlerWithResultFrame : AsyncFrame
        {
            private readonly Type? _handlerReturnType;
            private readonly Variable _result;

            public MaybeEndHandlerWithResultFrame(Variable result, Type? handlerReturnType)
            {
                uses.Add(result);
                _result = result;
                _handlerReturnType = handlerReturnType;
            }

            public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
            {
                writer.BlankLine();
                writer.WriteComment("Result continuation check for Result types");

                writer.Write($"BLOCK:if ({_result.Usage}.IsSuccess == false)");
                if (_handlerReturnType != null)
                    writer.Write(
                        _handlerReturnType != _result.VariableType
                            ? $"await context.EnqueueCascadingAsync(({GetFriendlyTypeName(_handlerReturnType)}){_result.Usage}.Error!).ConfigureAwait(false);"
                            : $"await context.EnqueueCascadingAsync({_result.Usage}).ConfigureAwait(false);");
                writer.Write("return;");
                writer.FinishBlock();
                writer.BlankLine();

                Next?.GenerateCode(method, writer);
            }
        }
        
        private class MaybeEndHandlerWithGenericResultFrame : AsyncFrame
        {
            private readonly Type? _handlerReturnType;
            private readonly Variable _result;

            public MaybeEndHandlerWithGenericResultFrame(Variable result, Type? handlerReturnType)
            {
                uses.Add(result);
                // Register a new variable for the success value of the Result<T>
                creates.Add(new Variable(result.VariableType.GetGenericArguments()[0], result.Usage + "SuccessValue"));

                var tupleInnerValues = GetTupleInnerValues(new Variable(result.VariableType.GetGenericArguments()[0],
                    result.Usage + "SuccessValue"));

                // If the Result<T> is a tuple, create variables for each inner value
                foreach (var innerValue in tupleInnerValues)
                    creates.Add(innerValue);


                creates.Add(new Variable(result.VariableType.GetGenericArguments()[0], result.Usage + "SuccessValue"));
                _result = result;
                _handlerReturnType = handlerReturnType;
            }

            public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
            {
                writer.BlankLine();
                writer.WriteComment("Result continuation check for Result<T> types");

                writer.Write($"BLOCK:if ({_result.Usage}.IsSuccess == false)");
                if (_handlerReturnType != null)
                    writer.Write(
                        _handlerReturnType != _result.VariableType
                            ? $"await context.EnqueueCascadingAsync(({GetFriendlyTypeName(_handlerReturnType)}){_result.Usage}.Error!).ConfigureAwait(false);"
                            : $"await context.EnqueueCascadingAsync({_result.Usage}).ConfigureAwait(false);");
                writer.Write("return;");
                writer.FinishBlock();

                writer.WriteComment("Extracting the success value from Result<T>");
                writer.WriteLine($"var {_result.Usage}SuccessValue = {_result.Usage}.Value;");

                Next?.GenerateCode(method, writer);
            }
        }
    }
}
