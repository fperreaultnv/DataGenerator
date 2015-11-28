﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataGenerator.Reflection;

namespace DataGenerator.Fluent
{
    public class ConfigurationBuilder
    {
        public ConfigurationBuilder(Configuration configuration)
        {
            Configuration = configuration;
        }


        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        /// <value>
        /// The current configuration.
        /// </value>
        public Configuration Configuration { get; }


        /// <summary>
        /// Include the <see cref="AppDomain" /> loaded assemblies as a source.
        /// </summary>
        /// <returns>
        /// A fluent <see langword="interface"/> to configure DataGenerator.
        /// </returns>
        public ConfigurationBuilder IncludeLoadedAssemblies()
        {
            Configuration.Assemblies.IncludeLoadedAssemblies();
            Configuration.ClearCache();
            return this;
        }

        /// <summary>
        /// Include the assembly from the specified type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to get assembly from.</typeparam>
        /// <returns>
        /// A fluent <see langword="interface"/> to configure DataGenerator.
        /// </returns>
        public ConfigurationBuilder IncludeAssemblyFor<T>()
        {
            return IncludeAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Include the specified <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The assembly to include.</param>
        /// <returns>
        /// A fluent <see langword="interface"/> to configure DataGenerator.
        /// </returns>
        public ConfigurationBuilder IncludeAssembly(Assembly assembly)
        {
            Configuration.Assemblies.IncludeAssembly(assembly);
            Configuration.ClearCache();
            return this;
        }

        /// <summary>
        /// Include the assemblies that contain the specified name.
        /// </summary>
        /// <param name="name">The name to compare.</param>
        /// <returns>
        /// A fluent <see langword="interface"/> to configure DataGenerator.
        /// </returns>
        public ConfigurationBuilder IncludeName(string name)
        {
            Configuration.Assemblies.IncludeName(name);
            Configuration.ClearCache();
            return this;
        }


        /// <summary>
        /// Exclude the assembly from the specified type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to get assembly from.</typeparam>
        /// <returns>
        /// A fluent <see langword="interface"/> to configure DataGenerator.
        /// </returns>
        public ConfigurationBuilder ExcludeAssemblyFor<T>()
        {
            return ExcludeAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Exclude the specified <see cref="Assembly" />.
        /// </summary>
        /// <param name="assembly">The assembly to exclude.</param>
        /// <returns>
        /// A fluent <see langword="interface"/> to configure DataGenerator.
        /// </returns>
        public ConfigurationBuilder ExcludeAssembly(Assembly assembly)
        {
            Configuration.Assemblies.ExcludeAssembly(assembly);
            Configuration.ClearCache();
            return this;
        }

        /// <summary>
        /// Exclude the assemblies that start with the specified name.
        /// </summary>
        /// <param name="name">The name to compare.</param>
        /// <returns>
        /// A fluent <see langword="interface"/> to configure DataGenerator.
        /// </returns>
        public ConfigurationBuilder ExcludeName(string name)
        {
            Configuration.Assemblies.ExcludeName(name);
            Configuration.ClearCache();
            return this;
        }



        public ConfigurationBuilder Entity<TEntity>(Action<ClassMappingBuilder<TEntity>> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var type = typeof(TEntity);
            var classMapping = GetClassMap(type);

            var mappingBuilder = new ClassMappingBuilder<TEntity>(classMapping);
            builder(mappingBuilder);

            return this;
        }


        public ConfigurationBuilder Profile<TProfile>() 
            where TProfile : IMappingProfile, new()
        {
            var profile = new TProfile();
            var type = profile.EntityType;
            var classMapping = GetClassMap(type);

            profile.Register(classMapping);

            return this;
        }


        private ClassMapping GetClassMap(Type type)
        {
            var classMapping = Configuration.Mapping.GetOrAdd(type, t =>
            {
                var typeAccessor = TypeAccessor.GetAccessor(t);
                var mapping = new ClassMapping(typeAccessor);
                return mapping;
            });

            return classMapping;
        }

    }
}
