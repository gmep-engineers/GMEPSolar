using System;
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
    private UpdateAction _Action;

    public Updateable()
    {
      _Action = UpdateAction.Read;
    }

    public UpdateAction Action
    {
      get { return _Action; }
      set
      {
        if (value != _Action)
        {
          if (value == UpdateAction.Create)
          {
            _Action = UpdateAction.Create;
          }
          else if (_Action != UpdateAction.Create || value == UpdateAction.Delete)
          {
            _Action = value;
          }
        }
      }
    }

    public void FinishCreate()
    {
      _Action = UpdateAction.Read;
    }
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
    private string _Kaic;
    private int _KaicId;
    private int _LoadVa;
    private int _VoltageId;
    private int _AmpId;
    public string PowerStationId;
    private string _Voltage;
    private string _Amp;

    public Lot(
      string Id,
      string Number,
      string Kaic,
      int LoadVa,
      string Voltage,
      string Amp,
      string PowerStationId,
      UpdateAction Action
    )
    {
      this.Id = Id;
      _Number = Number;
      this.Kaic = Kaic;
      _LoadVa = LoadVa;
      this.Voltage = Voltage;
      this.Amp = Amp;
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
    public int KaicId
    {
      get { return _KaicId; }
    }

    public string Kaic
    {
      get { return _Kaic; }
      set
      {
        if (value != _Kaic)
        {
          _Kaic = value;
          if (_Kaic == "22")
          {
            _KaicId = 1;
          }
          if (_Kaic == "42")
          {
            _KaicId = 2;
          }
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
    }

    public string Voltage
    {
      get { return _Voltage; }
      set
      {
        if (value != _Voltage)
        {
          _Voltage = value;
          if (_Voltage.StartsWith("120/208/3"))
          {
            _VoltageId = 1;
          }
          if (_Voltage.StartsWith("120/240/1"))
          {
            _VoltageId = 2;
          }
          if (_Voltage.StartsWith("277/480/3"))
          {
            _VoltageId = 3;
          }
          if (_Voltage.StartsWith("120/240/3"))
          {
            _VoltageId = 4;
          }
          if (_Voltage.StartsWith("120/208/1"))
          {
            _VoltageId = 5;
          }
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

    public string Amp
    {
      get { return _Amp; }
      set
      {
        if (value != _Amp)
        {
          _Amp = value;
          switch (_Amp)
          {
            case "100":
              _AmpId = 1;
              break;
            case "125":
              _AmpId = 2;
              break;
            case "150":
              _AmpId = 3;
              break;
            case "175":
              _AmpId = 4;
              break;
            case "200":
              _AmpId = 5;
              break;
            case "225":
              _AmpId = 6;
              break;
            case "250":
              _AmpId = 7;
              break;
            case "275":
              _AmpId = 8;
              break;
            case "300":
              _AmpId = 9;
              break;
            case "350":
              _AmpId = 10;
              break;
            case "400":
              _AmpId = 11;
              break;
          }
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

    public string GetProjectId(string projectName)
    {
      string query = @"SELECT id FROM projects WHERE project_name = @projectName";
      OpenConnection();
      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("@projectName", projectName);
      MySqlDataReader reader = command.ExecuteReader();
      string id = "";
      if (reader.Read())
      {
        id = reader.GetString("id");
      }
      reader.Close();
      return id;
    }

    public void CreatePowerStation(PowerStation powerStation, string projectId)
    {
      if (powerStation.Action != UpdateAction.Create)
      {
        return;
      }
      string query =
        @"
        INSERT IGNORE INTO power_stations (
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
      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("@id", powerStation.Id);
      command.Parameters.AddWithValue("@kwId", powerStation.KwId);
      command.Parameters.AddWithValue("@numBatts", powerStation.NumBatts);
      command.Parameters.AddWithValue("@projectId", projectId);
      command.ExecuteNonQuery();
      powerStation.FinishCreate();
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

    public void UpdatePowerStation(PowerStation powerStation)
    {
      if (powerStation.Action != UpdateAction.Update)
      {
        return;
      }
      string query =
        @"
        UPDATE power_stations
        SET
        kw_id = @kwId,
        num_batts = @numBatts,
        WHERE
        id = @id
        ";
      OpenConnection();

      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("@id", powerStation.Id);
      command.Parameters.AddWithValue("@kwId", powerStation.KwId);
      command.Parameters.AddWithValue("numBatts", powerStation.NumBatts);
      command.ExecuteNonQuery();
      powerStation.Action = UpdateAction.Read;

      CloseConnection();
    }

    public void DeletePowerStation(PowerStation powerStation)
    {
      if (powerStation.Action != UpdateAction.Delete)
      {
        return;
      }
      string query = @"DELETE FROM power_stations WHERE id = @id";
      OpenConnection();

      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("@Id", powerStation.Id);
      command.ExecuteNonQuery();

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
        INSERT IGNORE INTO lots (
        id,
        number,
        kaic_id,
        load_va,
        voltage_id,
        amp_id,
        power_station_id,
        project_id
        ) VALUES (
        @id,
        @number,
        @kaicId,
        @loadVa,
        @voltageId,
        @ampId,
        @powerStationId,
        @projectId
        )";
      OpenConnection();
      foreach (Lot lot in createLots)
      {
        if (lot.Number == null)
        {
          continue;
        }
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@id", lot.Id);
        command.Parameters.AddWithValue("@number", lot.Number);
        command.Parameters.AddWithValue("@kaicId", lot.KaicId);
        command.Parameters.AddWithValue("@loadVa", lot.LoadVa);
        command.Parameters.AddWithValue("@voltageId", lot.VoltageId);
        command.Parameters.AddWithValue("@ampId", lot.AmpId);
        command.Parameters.AddWithValue("@powerStationId", lot.PowerStationId);
        command.Parameters.AddWithValue("@projectId", projectId);
        command.ExecuteNonQuery();
        lot.FinishCreate();
      }
      CloseConnection();
    }

    public List<Lot> ReadLots(string powerStationId)
    {
      List<Lot> lots = new List<Lot>();
      string query =
        @"
        SELECT
        lots.id as lot_id,
        lots.number,
        kaic_ratings.rating as kaic_rating,
        lots.load_va,
        service_voltage_types.type as voltage_type,
        service_amp_ratings.rating as amp_rating
        FROM
        lots
        LEFT JOIN 
        service_voltage_types on service_voltage_types.id = lots.voltage_id
        LEFT JOIN
        service_amp_ratings on service_amp_ratings.id = lots.amp_id
        LEFT JOIN
        kaic_ratings on kaic_ratings.id = lots.kaic_id
        WHERE power_station_id = @powerStationId";
      OpenConnection();
      MySqlCommand command = new MySqlCommand(query, Connection);
      command.Parameters.AddWithValue("powerStationId", powerStationId);
      MySqlDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        lots.Add(
          new Lot(
            GetSafeString(reader, "lot_id"),
            GetSafeString(reader, "number"),
            GetSafeInt(reader, "kaic_rating").ToString(),
            GetSafeInt(reader, "load_va"),
            GetSafeString(reader, "voltage_type").Replace("PH", "\u03A6"),
            GetSafeInt(reader, "amp_rating").ToString(),
            powerStationId,
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
        kaic_id = @kaicId,
        load_va = @loadVa,
        voltage_id = @voltageId,
        amp_id = @ampId
        WHERE
        id = @id
        ";
      OpenConnection();
      foreach (Lot lot in updateLots)
      {
        MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@id", lot.Id);
        command.Parameters.AddWithValue("@number", lot.Number);
        command.Parameters.AddWithValue("@kaicId", lot.KaicId);
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
