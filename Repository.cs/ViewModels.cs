using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class NotesViewModel
    {
        public List<NoteViewModel> Notes { get; set; }
    }

    public class NoteViewModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
