using DO;

namespace Dal;
internal static class Config
{
    static readonly string s_data_config_xml = "data-config";
    internal static int NextTaskId { get => XMLTools.GetAndIncreaseNextId(s_data_config_xml, "NextTaskId"); }
    internal static int NextDependencyId { get => XMLTools.GetAndIncreaseNextId(s_data_config_xml, "NextDependencyId"); }
    internal static DateTime? StartDate { get => XMLTools.Get(s_data_config_xml, "StartDate", XMLTools.ToDateTimeNullable); set => XMLTools.Set(s_data_config_xml, "StartDate", value); }
    internal static DateTime? EndDate { get => XMLTools.Get(s_data_config_xml, "EndDate", XMLTools.ToDateTimeNullable); set => XMLTools.Set(s_data_config_xml, "EndDate", value); }
    internal static ProjectStatus? ProjectStatus { get => XMLTools.Get(s_data_config_xml, "ProjectStatus", XMLTools.ToEnumNullable<ProjectStatus>); set => XMLTools.Set(s_data_config_xml, "ProjectStatus", value); }
    internal static void ResetConfig()
    {
        XMLTools.Set(s_data_config_xml, "NextTaskId", 1);
        XMLTools.Set(s_data_config_xml, "NextDependencyId", 1);
        XMLTools.Set(s_data_config_xml, "ProjectStatus", DO.ProjectStatus.Planning);
        XMLTools.Set(s_data_config_xml, "StartDate", new DateTime?());
        XMLTools.Set(s_data_config_xml, "EndDate", new DateTime?());
    }

}
