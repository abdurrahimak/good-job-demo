
namespace CoreProject.Serilization
{
    public interface ISerilizationStrategy<TObjectType>
    {
        object Serialize(TObjectType obj);
        TObjectType Deserialize(object data);
    }
}