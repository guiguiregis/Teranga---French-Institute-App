using System.Collections.Generic;
using System.Linq;
public class TeamMembersData
{

    // Names
    static List<string> male_names = new List<string>()
    {
        "Babacar",
        "Abacar",
        "Mamadou",
        "Thierno",
        "Habibe",
        "Karim",
        "Macodou",
        "Mendy",
        "Djiby",
        "Lamine",
        "Abdou",
        "Moussa",
        "Cheikh",
        "Tidiane",
        "Sémou",
        "Moustapha",
        "Bass",
        "Vincent",
        "Malick",
        "Jean",
        "René"
    };

    static List<string> female_names = new List<string>()
    {
        "Fanta",
        "Binta",
        "Mame",
        "Fatou",
        "Lissa",
        "Aicha",
        "kiné",
        "Astou",
        "Aby",
        "Soda",
        "Nabou",
        "Isa",
        "Sofia",
        "Amina",
        "Awa",
        "Marie",
        "Adja",
        "Aida",
        "Maréme",
        "Rose",
        "Sylvie"

    };
    public static Names s_Names = new Names(){males = male_names, females = female_names };

    // Roles

    static Roles roles_1 = new Roles()
    {
        maxLevel = 6,
        titles = new List<string>()
        {
            "Stagiaire" , "Développeur" , "Chef de projet" , "Comptable externe" , "Community manager"
        }
    };
    static Roles roles_2 = new Roles()
    {
        maxLevel = 12,
        titles = new List<string>()
        {
            "Business developer", "Responsable communication", "Ressources humaines", "Directeur technique", "Chef de projet senior", "Développeur Senior",
            "Stagiaire" , "Développeur" , "Chef de projet" , "Comptable externe" , "Community manager",
        }
    };
    static Roles roles_3 = new Roles()
    {
        maxLevel = 900,
        titles = new List<string>()
        {
            "Assistante de direction", "Directeur des ressources humaines", "Directeur financier", "Directeur des opérations", "Consultant",
            "Business developer", "Responsable communication", "Ressources humaines", "Directeur technique", "Chef de projet senior", "Développeur Senior",
            "Stagiaire" , "Développeur" , "Chef de projet" , "Comptable externe" , "Community manager",
        }
    };

    public static List<Roles> s_Roles = new List<Roles>()
    {
        roles_1, roles_2, roles_3
    };


}
 

[System.Serializable]
public class Names
{
    public List<string> males;
    public List<string> females;
}

[System.Serializable]
public class Roles
{
    public int maxLevel = 6;
    public List<string> titles;
}
