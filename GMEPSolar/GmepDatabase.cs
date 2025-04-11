using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GMEPSolar
{
  public enum UpdateAction
  {
    Create,
    Read,
    Update,
    Delete,
  }

  public class Updateable
  {
    public UpdateAction Action;

    public Updateable() { }
  }

  public class PowerStation : Updateable
  {
    public string Id;
    private int _KwId;
    private int _NumBatts;
    public List<Lot> Lots;

    public PowerStation(string Id, int KwId, int NumBatts, UpdateAction Action)
    {
      this.Id = Id;
      _KwId = KwId;
      _NumBatts = NumBatts;
      this.Action = Action;
      Lots = new List<Lot>();
    }

    public int KwId
    {
      get { return _KwId; }
      set
      {
        if (value != _KwId)
        {
          _KwId = value;
          Action = UpdateAction.Update;
        }
      }
    }

    public int NumBatts
    {
      get { return _NumBatts; }
      set
      {
        if (value != _NumBatts)
        {
          _NumBatts = value;
          Action = UpdateAction.Update;
        }
      }
    }
  }

  public class Lot : Updateable
  {
    public string Id;
    private string _Number;
    private int _Kaic;
    private int _LoadVa;
    private int _VoltageId;
    private int _AmpId;
    public string PowerStationId;

    public Lot(
      string Id,
      string Number,
      int Kaic,
      int LoadVa,
      int VoltageId,
      int AmpId,
      string PowerStationId,
      UpdateAction Action
    )
    {
      this.Id = Id;
      _Number = Number;
      _Kaic = Kaic;
      _LoadVa = LoadVa;
      _VoltageId = VoltageId;
      _AmpId = AmpId;
      this.PowerStationId = PowerStationId;
      this.Action = Action;
    }

    public string Number
    {
      get { return _Number; }
      set
      {
        if (value != _Number)
        {
          _Number = value;
          Action = UpdateAction.Update;
        }
      }
    }

    public int Kaic
    {
      get { return _Kaic; }
      set
      {
        if (value != _Kaic)
        {
          _Kaic = value;
          Action = UpdateAction.Update;
        }
      }
    }

    public int LoadVa
    {
      get { return _LoadVa; }
      set
      {
        if (value != _LoadVa)
        {
          _LoadVa = value;
          Action = UpdateAction.Update;
        }
      }
    }

    public int VoltageId
    {
      get { return _VoltageId; }
      set
      {
        if (value != _VoltageId)
        {
          _VoltageId = value;
          Action = UpdateAction.Update;
        }
      }
    }

    public int AmpId
    {
      get { return _AmpId; }
      set
      {
        if (value != _AmpId)
        {
          _AmpId = value;
          Action = UpdateAction.Update;
        }
      }
    }
  }

  public class GmepDatabase
  {
    public string ConnectionString { get; set; }
    public static MySqlConnection Connection { get; set; }

    public GmepDatabase()
    {
      ConnectionString = Properties.Settings.Default.ConnectionString;
      Connection = new MySqlConnection(ConnectionString);
    }

    public void OpenConnection()
    {
      if (Connection.State == System.Data.ConnectionState.Closed)
      {
        Connection.Open();
      }
    }

    public void CloseConnection()
    {
      if (Connection.State == System.Data.ConnectionState.Open)
      {
        Connection.Close();
      }
    }

    string GetSafeString(MySqlDataReader reader, string fieldName)
    {
      int index = reader.GetOrdinal(fieldName);
      if (!reader.IsDBNull(index))
      {
        return reader.GetString(index);
      }
      return string.Empty;
    }

    int GetSafeInt(MySqlDataReader reader, string fieldName)
    {
      int index = reader.GetOrdinal(fieldName);
      if (!reader.IsDBNull(index))
      {
        return reader.GetInt32(index);
      }
      return 0;
    }

    public string GetProjectId(string projectNo)
    {
      string query = @"SELECT id FROM projects WHERE gmep_project_no = @projectNo";
      OpenConnection();
      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("@projectNo", projectNo);
      MySqlDataReader reader = command.ExecuteReader();
      string id = "";
      if (reader.Read())
      {
        id = reader.GetString("id");
      }
      reader.Close();
      return id;
    }

    public void CreatePowerStations(List<PowerStation> powerStations, string projectId)
    {
      List<PowerStation> createPowerStations = powerStations.FindAll(ps =>
        ps.Action == UpdateAction.Create
      );
      if (createPowerStations.Count == 0)
      {
        return;
      }
      string query =
        @"
        INSERT INTO power_stations (
        id,
        kw_id,
        num_batts,
        project_id
        ) VALUES (
        @id,
        @kwId,
        @numBatts,
        @projectId
        )";
      OpenConnection();
      foreach (PowerStation powerStation in createPowerStations)
      {
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@id", powerStation.Id);
        command.Parameters.AddWithValue("@kwId", powerStation.KwId);
        command.Parameters.AddWithValue("@numBatts", powerStation.NumBatts);
        command.Parameters.AddWithValue("@projectId", projectId);
        command.ExecuteNonQuery();
        powerStation.Action = UpdateAction.Read;
      }
      CloseConnection();
    }

    public List<PowerStation> ReadPowerStations(string projectId)
    {
      List<PowerStation> powerStations = new List<PowerStation>();
      string query =
        @"
        SELECT id, kw_id, num_batts FROM power_stations WHERE project_id = @projectId
        ";
      OpenConnection();
      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("projectId", projectId);
      MySqlDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        powerStations.Add(
          new PowerStation(
            GetSafeString(reader, "id"),
            GetSafeInt(reader, "kw_id"),
            GetSafeInt(reader, "num_batts"),
            UpdateAction.Read
          )
        );
      }
      CloseConnection();
      reader.Close();
      return powerStations;
    }

    public void UpdatePowerStations(List<PowerStation> powerStations)
    {
      List<PowerStation> updatePowerStations = powerStations.FindAll(ps =>
        ps.Action == UpdateAction.Update
      );
      if (updatePowerStations.Count == 0)
      {
        return;
      }
      string query =
        @"
        UPDATE powerstations
        SET
        kw_id = @kwId,
        num_batts = @numBatts,
        WHERE
        project_id = @projectId
        ";
      OpenConnection();
      foreach (PowerStation powerStation in updatePowerStations)
      {
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@kwId", powerStation.KwId);
        command.Parameters.AddWithValue("numBatts", powerStation.NumBatts);
        command.ExecuteNonQuery();
        powerStation.Action = UpdateAction.Read;
      }
      CloseConnection();
    }

    public void DeletePowerStations(List<PowerStation> powerStations)
    {
      List<PowerStation> deletePowerStations = powerStations.FindAll(ps =>
        ps.Action == UpdateAction.Delete
      );
      if (deletePowerStations.Count == 0)
      {
        return;
      }
      string query = @"DELETE FROM power_stations WHERE id = @id";
      OpenConnection();
      foreach (PowerStation powerStation in deletePowerStations)
      {
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@Id", powerStation.Id);
        command.ExecuteNonQuery();
      }
      CloseConnection();
    }

    public void CreateLots(List<Lot> lots, string projectId)
    {
      List<Lot> createLots = lots.FindAll(l => l.Action == UpdateAction.Create);
      if (createLots.Count == 0)
      {
        return;
      }
      string query =
        @"
        INSERT INTO lots (
        id,
        number,
        kaic,
        load_va,
        voltage_id,
        amp_id,
        power_station_id,
        project_id
        ) VALUES (
        @id,
        @number,
        @kaic,
        @loadVa,
        @voltageId,
        @ampId,
        @powerStationId,
        @projectId
        )";
      OpenConnection();
      foreach (Lot lot in createLots)
      {
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@id", lot.Id);
        command.Parameters.AddWithValue("@number", lot.Number);
        command.Parameters.AddWithValue("@kaic", lot.Kaic);
        command.Parameters.AddWithValue("@loadVa", lot.LoadVa);
        command.Parameters.AddWithValue("@voltageId", lot.VoltageId);
        command.Parameters.AddWithValue("@ampId", lot.AmpId);
        command.Parameters.AddWithValue("@powerStationId", lot.PowerStationId);
        command.Parameters.AddWithValue("@projectId", projectId);
        command.ExecuteNonQuery();
        lot.Action = UpdateAction.Read;
      }
      CloseConnection();
    }

    public List<Lot> ReadLots(string projectId)
    {
      List<Lot> lots = new List<Lot>();
      string query =
        @"
        SELECT
        id,
        number,
        kaic,
        load_va,
        voltage_id,
        amp_id,
        power_station_id
        FROM
        lots
        WHERE project_id = @projectId";
      OpenConnection();
      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("projectId", projectId);
      MySqlDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        lots.Add(
          new Lot(
            GetSafeString(reader, "id"),
            GetSafeString(reader, "number"),
            GetSafeInt(reader, "kaic"),
            GetSafeInt(reader, "load_va"),
            GetSafeInt(reader, "voltage_id"),
            GetSafeInt(reader, "amp_id"),
            GetSafeString(reader, "power_station_id"),
            UpdateAction.Read
          )
        );
      }
      CloseConnection();
      reader.Close();
      return lots;
    }

    public void UpdateLots(List<Lot> lots)
    {
      List<Lot> updateLots = lots.FindAll(l => l.Action == UpdateAction.Update);
      string query =
        @"
        UPDATE lots
        SET
        number = @number,
        kaic = @kaic,
        load_va = @loadVa,
        voltage_id = @voltageId,
        amp_id = @ampId
        ";
      OpenConnection();
      foreach (Lot lot in updateLots)
      {
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@number", lot.Number);
        command.Parameters.AddWithValue("@kaic", lot.Kaic);
        command.Parameters.AddWithValue("@loadVa", lot.LoadVa);
        command.Parameters.AddWithValue("@voltageId", lot.VoltageId);
        command.Parameters.AddWithValue("@ampId", lot.AmpId);
        command.ExecuteNonQuery();
        lot.Action = UpdateAction.Read;
      }
      CloseConnection();
    }

    public void DeleteLots(List<Lot> lots)
    {
      List<Lot> deleteLots = lots.FindAll(l => l.Action == UpdateAction.Delete);
      if (deleteLots.Count == 0)
      {
        return;
      }
      string query = @"DELETE FROM lots WHERE id = @id";
      OpenConnection();
      foreach (Lot lot in deleteLots)
      {
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@Id", lot.Id);
        command.ExecuteNonQuery();
      }
      CloseConnection();
    }
  }
}
