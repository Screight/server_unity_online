using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket_Server
{
    struct User
    {
        string m_name;
        int m_raceID;

        public string Name { get { return m_name; } set { m_name = value; } }
        public int RaceID { get { return m_raceID; } set { m_raceID = value; } }
    }

    struct Race
    {
        int m_id;
        string m_name;
        int m_maxHealth;
        int m_damage;
        int m_speed;
        int m_jumpForce;
        int m_fireRate;

        public int ID { get { return m_id; } set { m_id = value; } }
        public int MaxHealth { get { return m_maxHealth; } set { m_maxHealth = value; } }
        public int Damage { get { return m_damage; } set { m_damage = value; } }
        public int JumpForce { get { return m_jumpForce; } set { m_jumpForce = value; } }
        public int Speed { get { return m_speed; } set { m_speed = value; } }
        public int FireRate { get { return m_fireRate; } set { m_fireRate = value; } }
        public string Name { get { return m_name; } set { m_name = value; } }
    }
}
