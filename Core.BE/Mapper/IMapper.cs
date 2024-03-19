namespace Emeint.Core.BE.Mapper
{
    public interface IEndUserMapper<DB, VM>
    {
        DB MapFromEndUserVMToDbModel(VM source);

        VM MapFromDbModelToEndUserVM(DB source);

    }

    public interface IAdminMapper<DB, VM>
    {

        DB MapFromAdminVMToDbModel(VM source);

        VM MapFromDbModelToAdminVM(DB source);
    }
}
