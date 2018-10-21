namespace OrdersCollector.Domain.Local
{
    namespace Commands.V1
    {
        public class AddLocal
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class RenameLocal
        {
            public string Id { get; set; }

            public string NewName { get; set; }
        }

        public class RemoveLocal
        {
            public string Id { get; set; }
        }

        public class AddLocalAlias
        {
            public string LocalId { get; set; }

            public string Alias { get; set; }
        }

        public class RemoveLocalAlias
        {
            public string LocalId { get; set; }

            public string Alias { get; set; }
        }
    }
}