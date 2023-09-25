namespace RealWorldProjectUnitTest.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //virtual keyword kullanıyoruz çogu zaman özellikle lazy loading yapmak istediğimiz zaman
        //entity hazır edebilmesi açısından şu aşamada gerekmediği için eklemedik
        public ICollection<Product> Products { get; set; }
    }
}
