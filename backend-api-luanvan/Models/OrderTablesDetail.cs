﻿using System;
using System.Collections.Generic;

namespace backend_api_luanvan.Models;

public partial class OrderTablesDetail
{
    public int OrderTablesDetailsId { get; set; }

    public long? OrderTableId { get; set; }

    public int? TableId { get; set; }

    public virtual OrderTable? OrderTable { get; set; }

    public virtual Table? Table { get; set; }
}
