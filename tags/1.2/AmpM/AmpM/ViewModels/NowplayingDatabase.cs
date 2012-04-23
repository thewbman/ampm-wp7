using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using Wintellect.Sterling.Database;
using Wintellect.Sterling.Events;
using Wintellect.Sterling.Exceptions;
using Wintellect.Sterling.Indexes;
using Wintellect.Sterling.Keys;
using Wintellect.Sterling.Serialization;

namespace AmpM
{
    public class NowplayingDatabase : BaseDatabaseInstance
    {

        protected override List<ITableDefinition> RegisterTables()
        {
            return new List<ITableDefinition>
                       {
                           CreateTableDefinition<DataItemViewModel, string>(testModel => testModel.ItemKey)
                               .WithIndex<DataItemViewModel, int, int>("ItemId", t => t.ItemId)
                               .WithIndex<DataItemViewModel, string, string>("Type", t => t.Type)
                       };
        }
    }    
}
