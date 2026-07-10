// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

#if BETA
using System;
using System.Collections.Generic;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Builds multi-instruction DSL pipelines by combining existing single-instruction builders.
    /// </summary>
    public class AlchemyPipelineBuilder
    {
        private readonly List<string> _segments = new List<string>();

        /// <summary>
        /// Adds a fully configured single-instruction builder to the pipeline.
        /// </summary>
        /// <typeparam name="TParam">The parameter type of the builder.</typeparam>
        /// <param name="function">The builder that produces a single DSL instruction.</param>
        /// <returns>The same pipeline instance for chaining.</returns>
        public AlchemyPipelineBuilder Add<TParam>(IAlchemyFunction<TParam> function)
        {
            var executable = function.Build();
            // 舊式建構器總是產生一個指令，但為了擴展性，我們遍歷所有
            foreach (var dsl in executable.GetDsls())
            {
                _segments.Add(dsl);
            }
            return this;
        }

        /// <summary>
        /// Builds the pipeline into an executable that can run all instructions.
        /// </summary>
        public AlchemyExecutable Build()
        {
            if (_segments.Count == 0)
                throw new InvalidOperationException("Pipeline contains no instructions. Add at least one segment.");
            return new AlchemyExecutable(_segments.ToArray());
        }
    }
}
#endif
