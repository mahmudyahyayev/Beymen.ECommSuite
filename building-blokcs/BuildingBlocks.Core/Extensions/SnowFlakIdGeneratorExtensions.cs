using DotNetEnv;
using IdGen;

namespace BuildingBlocks.Core.Extensions
{
    public static class SnowFlakIdGenerator
    {
        private static readonly IdGenerator _generator;

        static SnowFlakIdGenerator()
        {
            var generatorId = Env.GetInt("GENERATOR_ID", 0);
            var epoch = new DateTime(2023, 10, 12, 0, 0, 0, DateTimeKind.Local);
            var structure = new IdStructure(45, 2, 16);
            var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));
            _generator = new IdGenerator(generatorId, options);
        }

        public static long NewId()
        {
            return _generator.CreateId();
        }
    }
}
