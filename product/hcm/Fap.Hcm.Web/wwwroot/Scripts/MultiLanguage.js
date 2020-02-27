﻿
//JS资源多语， 由系统自动生成， 请勿修改
var MultiLangHelper = (function () {
    var resLang = 'ZhCn';
    var resArray = { 
      'save': { 'ZhCn': '保存', 'ZhTW': '', 'En': 'save', 'Ja': '' },
      'previous': { 'ZhCn': '上一条', 'ZhTW': '', 'En': 'previous', 'Ja': '' },
      'next': { 'ZhCn': '下一条', 'ZhTW': '', 'En': 'next', 'Ja': '' },
      'save_add': { 'ZhCn': '保存并新增', 'ZhTW': '', 'En': 'save and add', 'Ja': '' },
      'select_row': { 'ZhCn': '请选中数据操作', 'ZhTW': '', 'En': 'please selected row', 'Ja': '' },
      'cancel': { 'ZhCn': '取消', 'ZhTW': '', 'En': 'cancel', 'Ja': '' },
      'next_step': { 'ZhCn': '下一步', 'ZhTW': '', 'En': 'next step', 'Ja': '' },
      'set_property': { 'ZhCn': '设置属性值', 'ZhTW': '', 'En': 'set property', 'Ja': '' },
      'finish': { 'ZhCn': '完成', 'ZhTW': '', 'En': 'finish', 'Ja': '' },
      'previous_step': { 'ZhCn': '上一步', 'ZhTW': '', 'En': 'previous step', 'Ja': '' },
      'view': { 'ZhCn': '查看', 'ZhTW': '', 'En': 'view', 'Ja': '' },
      'saveas_common_query': { 'ZhCn': '保存为常用查询', 'ZhTW': '', 'En': 'save as common query', 'Ja': '' },
      'common_query': { 'ZhCn': '常用查询', 'ZhTW': '', 'En': 'common query', 'Ja': '' },
      'ok': { 'ZhCn': '确定', 'ZhTW': '', 'En': 'ok', 'Ja': '' },
      'dowload_template': { 'ZhCn': '下载模板', 'ZhTW': '', 'En': 'download template', 'Ja': '' },
      'download_empty_template': { 'ZhCn': '下载空模板', 'ZhTW': '', 'En': 'download empty template', 'Ja': '' },
      'import_data': { 'ZhCn': '导入数据', 'ZhTW': '', 'En': 'import data', 'Ja': '' },
      'download_data_template': { 'ZhCn': '下载数据模板', 'ZhTW': '', 'En': 'download template', 'Ja': '' },
      'export_item': { 'ZhCn': '导出项', 'ZhTW': '', 'En': 'export item', 'Ja': '' },
      'pending_item': { 'ZhCn': '待选项', 'ZhTW': '', 'En': 'pending item', 'Ja': '' },
      'global_oper_enter': { 'ZhCn': '确定', 'ZhTW': '', 'En': 'OK', 'Ja': '' },
      'confirm_delete': { 'ZhCn': '确定要删除选中的数据吗?', 'ZhTW': '', 'En': 'confirm delete?', 'Ja': '' },
      'upload_template': { 'ZhCn': '上传模板', 'ZhTW': '', 'En': 'upload template', 'Ja': '' },
      'close': { 'ZhCn': '关闭', 'ZhTW': '', 'En': 'close', 'Ja': '' },
      'first_row': { 'ZhCn': '已到达第一条数据', 'ZhTW': '', 'En': 'first row', 'Ja': '' },
      'end_row': { 'ZhCn': '已到达最后一条数据', 'ZhTW': '', 'En': 'last row', 'Ja': '' },
      'select_property': { 'ZhCn': '选择编辑属性', 'ZhTW': '', 'En': 'select property', 'Ja': '' },
      'annex': { 'ZhCn': '附件', 'ZhTW': '', 'En': 'annex', 'Ja': '' },
      'remove_all': { 'ZhCn': '移除所有', 'ZhTW': '', 'En': 'remove all', 'Ja': '' },
      'filter': { 'ZhCn': '过滤', 'ZhTW': '', 'En': 'filter', 'Ja': '' },
      'remove_selected': { 'ZhCn': '移除选中', 'ZhTW': '', 'En': 'remove selected', 'Ja': '' },
      'move_all': { 'ZhCn': '移动所有', 'ZhTW': '', 'En': 'Move all', 'Ja': '' },
      'show_all': { 'ZhCn': '显示全部', 'ZhTW': '', 'En': 'Show all', 'Ja': '' },
      'filtered': { 'ZhCn': '过滤到', 'ZhTW': '', 'En': 'Filtered', 'Ja': '' },
      'move_selected': { 'ZhCn': '移动选中', 'ZhTW': '', 'En': 'move selected', 'Ja': '' },
      'from': { 'ZhCn': '从', 'ZhTW': '', 'En': 'from', 'Ja': '' },
      'empty_list': { 'ZhCn': '空列表', 'ZhTW': '', 'En': 'Empty list', 'Ja': '' },
      'view_annex': { 'ZhCn': '查看附件', 'ZhTW': '', 'En': 'View annex', 'Ja': '' },
      'information': { 'ZhCn': '提示信息', 'ZhTW': '', 'En': 'Information', 'Ja': '' },
      'add_user': { 'ZhCn': '添加用户', 'ZhTW': '', 'En': 'add user', 'Ja': '' },
      'code': { 'ZhCn': '编码', 'ZhTW': '', 'En': '', 'Ja': '' },
      'name': { 'ZhCn': '名称', 'ZhTW': '', 'En': '', 'Ja': '' },
      'is_enabled_workflow': { 'ZhCn': '是否要启用该流程', 'ZhTW': '', 'En': '', 'Ja': '' },
      'is_disabled_workflow': { 'ZhCn': '是否要禁用该流程', 'ZhTW': '', 'En': '', 'Ja': '' },
      'is_upgrade_workflow': { 'ZhCn': '是否要升级该流程', 'ZhTW': '', 'En': '', 'Ja': '' },
      'workflow_designer': { 'ZhCn': '流程设计器', 'ZhTW': '', 'En': '', 'Ja': '' },
      'success': { 'ZhCn': '授权成功！', 'ZhTW': '', 'En': '', 'Ja': '' },
      'ess_userprofile_page_changphoto': { 'ZhCn': '更换照片', 'ZhTW': '', 'En': '', 'Ja': '' },
      'global_file_photo_updated': { 'ZhCn': '照片已更新', 'ZhTW': '', 'En': '', 'Ja': '' },
      'replace_photo': { 'ZhCn': '更换照片', 'ZhTW': '', 'En': '', 'Ja': '' },
      'global_oper_cancel': { 'ZhCn': '取消', 'ZhTW': '', 'En': '', 'Ja': '' },
      'clear': { 'ZhCn': '清空!', 'ZhTW': '', 'En': '', 'Ja': '' }
    };
    var Helper = {};
    //初始化语种
    Helper.initLang = function (lang) {
        resLang = lang;
    };
    //获得资源多语
    Helper.getResName = function (code, defaultName) {
        if (resArray[code] === undefined) {
            $.post(basePath +'/Api/Core/MultiLanguage',{ langKey: code, langValue: defaultName }, function (rv) {

            });
            return defaultName;
        }
        return resArray[code][resLang] || defaultName;
    };

    return Helper;
})();