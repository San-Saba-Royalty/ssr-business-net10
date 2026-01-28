using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class CadDataRepository
{
    private string GetSearchClause(string crit, string columnName)
    {
        var sql = new StringBuilder();

        if (!string.IsNullOrEmpty(crit))
        {
            sql.Append(" AND (");
            var searchItems = crit.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < searchItems.Length; i++)
            {
                if (i > 0)
                {
                    sql.Append(" AND ");
                }
                // Determine if we need to quote the identifier in case it has hyphens etc.
                // Legacy code wrapped column names in brackets [COUNTY-NUMBER].
                sql.AppendFormat("[{0}] LIKE '%{1}%'", columnName, searchItems[i].ToUpper().Replace("'", "''"));
            }
            sql.Append(") ");
        }

        return sql.ToString();
    }

    private string GetSearchClauseListOr(List<string> crit, string columnName)
    {
        var sql = new StringBuilder();

        if (crit != null && crit.Count > 0)
        {
            // Filter out empty strings
            var validCrit = crit.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (validCrit.Count > 0)
            {
                sql.Append(" AND (");
                for (int i = 0; i < validCrit.Count; i++)
                {
                    if (i > 0)
                    {
                        sql.Append(" OR ");
                    }

                    // Logic from GetSearchClause2 in legacy code:
                    var searchItems = validCrit[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    sql.Append("(");
                    for (int j = 0; j < searchItems.Length; j++)
                    {
                        if (j > 0) sql.Append(" AND ");
                        sql.AppendFormat("[{0}] LIKE '%{1}%'", columnName, searchItems[j].ToUpper().Replace("'", "''"));
                    }
                    sql.Append(")");
                }
                sql.Append(") ");
            }
        }

        return sql.ToString();
    }

    public List<CadData> Search(CadData crit, string displayName, string tableName, string connectionString)
    {
        var cadDataList = new List<CadData>();

        using (var cnn = new SqlConnection(connectionString))
        {
            var sql = new StringBuilder();

            sql.Append("SELECT [COUNTY-NUMBER], [RRC-LEASE], [LEASE], [OWNER], [NAME], [ADDR1], [ADDR2], [CITY], [ST], [ZIP5], [ZIP4], [LEASENAME], [OPERATOR], [FIELD], [INTYP], [INTEREST], [ABSTRACT], [SURVEY], [VALUE], [ACRES] ");
            sql.AppendFormat("FROM [{0}] ", tableName);
            sql.Append("WHERE 1 = 1 ");

            sql.Append(GetSearchClauseListOr(crit.CountyNumberList, "COUNTY-NUMBER"));
            // Legacy code also checked crit.CountyNumber here, but CountyNumberList seems to be the primary for searches.
            if (!string.IsNullOrEmpty(crit.CountyNumber))
            {
                sql.Append(GetSearchClause(crit.CountyNumber, "COUNTY-NUMBER"));
            }

            sql.Append(GetSearchClause(crit.RrcLease, "RRC-LEASE"));
            sql.Append(GetSearchClause(crit.Lease, "LEASE"));
            sql.Append(GetSearchClause(crit.Owner, "OWNER"));
            sql.Append(GetSearchClause(crit.Name, "NAME"));
            sql.Append(GetSearchClause(crit.Addr1, "ADDR1"));
            sql.Append(GetSearchClause(crit.Addr2, "ADDR2"));
            sql.Append(GetSearchClause(crit.City, "CITY"));
            sql.Append(GetSearchClause(crit.St, "ST"));
            sql.Append(GetSearchClause(crit.Zip5, "ZIP5"));
            // Zip4 logic was commented out in legacy
            // sql.Append(GetSearchClause(crit.Zip4, "ZIP4"));
            sql.Append(GetSearchClause(crit.LeaseName, "LEASENAME"));
            sql.Append(GetSearchClause(crit.OperatorName, "OPERATOR"));
            sql.Append(GetSearchClause(crit.Field, "FIELD"));
            sql.Append(GetSearchClause(crit.Intyp, "INTYP"));
            // Interest logic commented out in legacy
            // sql.Append(GetSearchClause(crit.Interest, "INTEREST"));
            sql.Append(GetSearchClause(crit.Abstract, "ABSTRACT"));
            sql.Append(GetSearchClause(crit.Survey, "SURVEY"));
            // Value and Acres logic commented out in legacy

            try
            {
                using (var cmd = new SqlCommand(sql.ToString(), cnn))
                {
                    cnn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var itm = new CadData();
                            itm.TableDisplayName = displayName;

                            // Using ordinal positions based on the SELECT list order for performance, similar to legacy
                            // SELECT [COUNTY-NUMBER], [RRC-LEASE], [LEASE], [OWNER], [NAME], [ADDR1], [ADDR2], [CITY], [ST], [ZIP5], [ZIP4], [LEASENAME], [OPERATOR], [FIELD], [INTYP], [INTEREST], [ABSTRACT], [SURVEY], [VALUE], [ACRES]
                            int pos = 0;
                            if (!rdr.IsDBNull(pos)) itm.CountyNumber = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.RrcLease = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Lease = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Owner = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Name = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Addr1 = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Addr2 = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.City = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.St = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Zip5 = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Zip4 = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.LeaseName = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.OperatorName = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Field = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Intyp = rdr.GetString(pos); pos++;

                            // Interest is double in SELECT
                            if (!rdr.IsDBNull(pos)) itm.InterestValue = rdr.GetDouble(pos); pos++;
                            itm.Interest = itm.InterestValue?.ToString() ?? string.Empty;

                            if (!rdr.IsDBNull(pos)) itm.Abstract = rdr.GetString(pos); pos++;
                            if (!rdr.IsDBNull(pos)) itm.Survey = rdr.GetString(pos); pos++;

                            // Value is double
                            if (!rdr.IsDBNull(pos)) itm.Value = rdr.GetDouble(pos).ToString(); pos++;

                            // Acres is double
                            if (!rdr.IsDBNull(pos)) itm.Acres = rdr.GetDouble(pos).ToString(); pos++;

                            cadDataList.Add(itm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In production code we should probably log this
                // For now, rethrow or handle as empty list if connection fails?
                // Legacy code doesn't explicitly handle connection failure inside the Read loop context well, but we'll bubble up up the exception.
                throw new Exception($"Error querying CAD table {tableName}: {ex.Message}", ex);
            }
        }

        return cadDataList;
    }
}
