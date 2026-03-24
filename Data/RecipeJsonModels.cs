using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CompositeBuildables
{
    [DataContract]
    public class RecipeIngredientEntry
    {
        [DataMember]
        public string TechType { get; set; }

        [DataMember]
        public int Amount { get; set; }
    }

    [DataContract]
    public class RecipeComplexitySet
    {
        [DataMember]
        public List<RecipeIngredientEntry> Simple { get; set; } = new List<RecipeIngredientEntry>();

        [DataMember]
        public List<RecipeIngredientEntry> Standard { get; set; } = new List<RecipeIngredientEntry>();

        [DataMember]
        public List<RecipeIngredientEntry> Complex { get; set; } = new List<RecipeIngredientEntry>();
    }

    [DataContract]
    public class RecipeEntry
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public RecipeComplexitySet Recipes { get; set; } = new RecipeComplexitySet();
    }

    [DataContract]
    public class RecipeFileRoot
    {
        [DataMember]
        public List<RecipeEntry> Buildables { get; set; } = new List<RecipeEntry>();
    }
}