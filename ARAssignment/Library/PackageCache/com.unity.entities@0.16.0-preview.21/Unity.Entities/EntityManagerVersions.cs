using System;
using Unity.Collections;

namespace Unity.Entities
{
    public unsafe partial struct EntityManager
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the version number of the specified component type.
        /// </summary>
        /// <remarks>This version number is incremented each time there is a structural change involving the specified
        /// type of component. Such changes include creating or destroying entities that have this component and adding
        /// or removing the component type from an entity. Shared components are not covered by this version;
        /// see <see cref="GetSharedComponentOrderVersion{T}(T)"/>.
        ///
        /// Version numbers can overflow. To compare if one version is more recent than another use a calculation such as:
        ///
        /// <code>
        /// bool VersionBisNewer = (VersionB - VersionA) > 0;
        /// </code>
        /// </remarks>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The current version number.</returns>
        [BurstCompatible(GenericTypeArguments = new[] { typeof(BurstCompatibleComponentData) })]
        public int GetComponentOrderVersion<T>()
        {
            var access = GetCheckedEntityDataAccess();
            var ecs = access->EntityComponentStore;
            return ecs->GetComponentTypeOrderVersion(TypeManager.GetTypeIndex<T>());
        }

        /// <summary>
        /// Gets the version number of the specified shared component.
        /// </summary>
        /// <remarks>
        /// This version number is incremented each time there is a structural change involving entities in the chunk of
        /// the specified shared component. Such changes include creating or destroying entities or anything that changes
        /// the archetype of an entity.
        ///
        /// Version numbers can overflow. To compare if one version is more recent than another use a calculation such as:
        ///
        /// <code>
        /// bool VersionBisNewer = (VersionB - VersionA) > 0;
        /// </code>
        /// </remarks>
        /// <param name="sharedComponent">The shared component instance.</param>
        /// <typeparam name="T">The shared component type.</typeparam>
        /// <returns>The current version number.</returns>
        [NotBurstCompatible]
        public int GetSharedComponentOrderVersion<T>(T sharedComponent) where T : struct, ISharedComponentData
        {
            var access = GetCheckedEntityDataAccess();
            var mcs = access->ManagedComponentStore;
            return mcs.GetSharedComponentVersion(sharedComponent);
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------

        internal uint GetChunkVersionHash(Entity entity)
        {
            var access = GetCheckedEntityDataAccess();
            var ecs = access->EntityComponentStore;

            if (!ecs->Exists(entity))
                return 0;

            var chunk = ecs->GetChunk(entity);
            var typeCount = chunk->Archetype->TypesCount;

            uint hash = 0;
            for (int i = 0; i < typeCount; ++i)
            {
                hash += chunk->GetChangeVersion(i);
            }

            return hash;
        }
    }
}
