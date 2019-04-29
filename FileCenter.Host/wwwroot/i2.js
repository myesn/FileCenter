$(function () {
    //绑定 页面
    $('#file').on('change', function () {
        //for (var i = 0; i < files.length; i++) {
        //    var file = files[i];
        //    var operate = new uploadOperate(file);
        //}
        $('#output').text('begin');
        $('#uploadBtn').text('正在上传...').prop('disabled', true);
        upload(this.files[0]);
    });

});

function upload(file, index) {
    index = index || 0;

    var totalSize = file.size; //总大小 chunkSize = 4 * 1024 * 1024, 
    var chunkSize = 4 * 1024 * 1024;//以4MB为一个分片
    var totalChunk = Math.ceil(totalSize / chunkSize); //总片数

    // 由后台判断
    //if (index === totalChunk) {
    //    return;
    //}

    //计算每一片的起始与结束位置
    var start = index * chunkSize;
    var end = Math.min(totalSize, start + chunkSize);

    //构造一个表单，FormData是HTML5新增的
    var form = new FormData();
    form.append('data', file.slice(start, end)); //slice方法用于切出文件的一部分
    form.append('lastModified', file.lastModified);
    form.append('fileName', file.name);
    form.append('total', totalChunk); //总片数
    form.append('index', index); //当前是第几片

    //ajax提交文件
    $.ajax({
        url: 'file',
        type: 'post',
        data: form,
        processData: false, //很重要，告诉jquery不要对form进行处理
        contentType: false, //很重要，指定为false才能形成正确的Content-Type
        success: function (result) {
            if (result) {
                if (result.merged) {
                    $('#output').text('上传成功');
                    $('#uploadBtn').text('点击上传').prop('disabled', false);;
                    return;
                }

                var nextIndex = result.index + 1;
                var progress = Math.ceil(nextIndex * 100 / totalChunk);

                $('#output').text(progress + '%');

                upload(file, nextIndex);
            }
        }
    });
}