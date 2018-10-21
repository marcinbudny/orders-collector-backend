namespace OrdersCollector.Domain.Local
{
    namespace Events.V1
    {
        public class LocalAdded
        {
            public string Id { get; }
        
            public string Name { get; }

            public LocalAdded(string id, string name)
            {
                Name = name;
                Id = id;
            }
        }

        public class LocalRenamed
        {
            public string Id { get; }
        
            public string NewName { get; }

            public LocalRenamed(string id, string newName)
            {
                Id = id;
                NewName = newName;
            }
        }

        public class LocalRemoved
        {
            public string Id { get; }

            public LocalRemoved(string id)
            {
                Id = id;
            }
        }

        public class LocalAliasAdded
        {
            public string LocalId { get; }

            public string Alias { get; }

            public LocalAliasAdded(string localId, string @alias)
            {
                LocalId = localId;
                Alias = alias;
            }
        }

        public class LocalAliasRemoved
        {
            public string LocalId { get; }

            public string Alias { get; }

            public LocalAliasRemoved(string localId, string @alias)
            {
                LocalId = localId;
                Alias = alias;
            }
        }
    }
}