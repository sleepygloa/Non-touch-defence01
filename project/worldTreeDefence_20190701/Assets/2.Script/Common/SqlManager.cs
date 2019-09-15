using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class SqlManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //connection();
        //Select("SELECT * FROM TB_GAME_DEFENCE_TREE_WORLD_HERO;");

    }


    MySqlConnection mysqlConnection = null;
    private string host = "http://61.252.235.153:18081";
    private string database = "seonhoblog";
    private string userId = "sleepygloa";
    private string password = "gkstna88";

    public void Select(string text)
    {

        Debug.Log("1");
        if (mysqlConnection != null)
        {
            Debug.Log("2");
            //MySqlCommand cmd = mysqlConnection.CreateCommand();
            //cmd.CommandText = text;
            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = mysqlConnection;
            cmd.CommandText = text;
            Debug.Log("3");
            MySqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            Debug.Log("4");

            disConnection();

            Debug.Log(text);
            //Debug.Log()

            int i = 0;
            while (rdr.Read())
            {
                Debug.Log("-------------------");
                Debug.Log(rdr[i]);
                i++;
            }
            rdr.Close();



        }
    }


    private void connection() { 
        if(mysqlConnection == null) {
            try {

                string connectionString = "Server=" + host + ";Database=" + database + ";UId=" + userId + ";Pwd=" + password;

                mysqlConnection = new MySqlConnection(connectionString);
                mysqlConnection.Open();
            
            }catch(MySqlException ex) {
                Debug.Log("Mysql connection Exception" + ex);
            }
        }
        Debug.Log("DB Connected");
    }

    private void disConnection() { 
        if(mysqlConnection != null) {
            mysqlConnection.Close();
        }
        Debug.Log("DB DisConnected");
    }

    public void Insert() {
        string commandText = string.Format("SELECT * FROM TB_GAME_DEFENCE_TREE_WORLD_HERO");

        if(mysqlConnection != null) {
            MySqlCommand command = mysqlConnection.CreateCommand();
            command.CommandText = commandText;

            try {
                command.ExecuteNonQuery();
            }catch(System.Exception ex) {
                Debug.LogError("MySql error : " + ex.ToString());
            }

        }
    }



}
