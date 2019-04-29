//封装 单个文件上传实例
(function () {
    var url = 'ws://localhost:5000/ws';
    //指定上传文件，创建上传操作对象
    function uploadOperate(file) {
        var _this = this;
        this.reader = new FileReader();//读取文件对象
        this.step = 1024 * 4;//每次读取文件字节数
        this.curLoaded = 0; //当前读取位置
        this.file = file;   //当前文件对象
        this.enableRead = true; //指定是否可读取,
        this.total = file.size;  //当前文件总大小
        this.startTime = new Date(); //开始读取时间
        //创建显示
        this.createItem();
        this.initWebSocket(function () {
            _this.bindReader();
        });
        console.info('文件大小：' + this.total);
    }
    uploadOperate.prototype = {
        //绑定读取事件
        bindReader: function () {
            var _this = this;
            var reader = this.reader;
            var ws = this.ws;
            reader.onload = function (e) {
                //判断是否能再次读取
                if (_this.enableRead == false) return;
                //根据当前缓冲区 控制读取速度
                if (ws.bufferedAmount >= _this.step * 20) {
                    setTimeout(function () {
                        _this.loadSuccess(e.loaded);
                    }, 5);
                    console.info('---->进入等待');
                } else {
                    _this.loadSuccess(e.loaded);
                }
            }
            //开始读取
            _this.readBlob();
        },
        //读取成功，操作处理
        loadSuccess: function (loaded) {
            var _this = this;
            var ws = _this.ws;
            //使用WebSocket 将二进制输出上传到服务器
            var blob = _this.reader.result;

            if (_this.curLoaded <= 0) {
                var filename = _this.file.name;
                var lastIndexOfDot = filename.lastIndexOf('.') + 1;
                var ext = filename.substr(lastIndexOfDot, filename.length - lastIndexOfDot);
                ws.send(JSON.stringify({
                    msg: 'begin',
                    ext: ext,
                    total: _this.file.size
                }));
            }
            if (blob.byteLength == 0)
                return;

            ws.send(blob);
            //当前发送完成，继续读取
            _this.curLoaded += loaded;
            if (_this.curLoaded < _this.total) {
                _this.readBlob();
            } else {
                //发送读取完成
                ws.send(JSON.stringify({
                    msg: 'done'
                }));
                //读取完成
                console.log('总共上传：' + _this.curLoaded + ',总共用时：' + (new Date().getTime() - _this.startTime.getTime()) / 1000);
            }
            //显示进度等
            _this.showProgress();
        },
        //创建显示项
        createItem: function () {
            var _this = this;
            var blockquote = document.createElement('blockquote');
            var abort = document.createElement('input');
            abort.type = 'button';
            abort.value = '中止';
            abort.onclick = function () {
                _this.stop();
            };
            blockquote.appendChild(abort);

            var containue = document.createElement('input');
            containue.type = 'button';
            containue.value = '继续';
            containue.onclick = function () {
                _this.containue();
            };
            blockquote.appendChild(containue);

            var progress = document.createElement('progress');
            progress.style.width = '400px';
            progress.max = 100;
            progress.value = 0;
            blockquote.appendChild(progress);
            _this.progressBox = progress;

            var status = document.createElement('p');
            status.id = 'Status';
            blockquote.appendChild(status);
            _this.statusBox = status;

            document.getElementById('bodyOne').appendChild(blockquote);
        },
        //显示进度
        showProgress: function () {
            var _this = this;
            var percent = (_this.curLoaded / _this.total) * 100;
            _this.progressBox.value = percent;
            _this.statusBox.innerHTML = percent;
        },
        //执行读取文件
        readBlob: function () {
            var blob = this.file.slice(this.curLoaded, this.curLoaded + this.step);
            this.reader.readAsArrayBuffer(blob);
        },
        //中止读取
        stop: function () {
            this.enableRead = false;
            this.reader.abort();
            console.log('读取中止，current Loaded:' + this.curLoaded);
        },
        //继续读取
        containue: function () {
            this.enableRead = true;
            this.readBlob();
            console.log('读取继续，current Loaded:' + this.curLoaded);
        },
        //初始化 绑定创建连接
        initWebSocket: function (onSuccess) {
            var _this = this;
            var ws = this.ws = new WebSocket(url); //初始化上传对象
            ws.onopen = function () {
                console.log('connect创建成功');
                if (onSuccess)
                    onSuccess();
            }
            ws.onmessage = function (e) {
                var data = e.data;
                if (isNaN(data) == false) {
                    console.info('后台接收成功：' + data);
                } else {
                    console.info(data);
                }
            }
            ws.onclose = function (e) {
                //中止读取
                _this.stop();
                console.log('connect已经断开');
            }
            ws.onerror = function (e) {
                //中止读取
                _this.stop();
                console.log('发生异常：' + e.message);
            }
        }
    };
    window.uploadOperate = uploadOperate;
})();
/*
* 测试WebSocket多文件上传
* 上传速度取决于 每次send() 的数据大小 ，Google之所以相对比较慢，是因为每次send的数据量太小
*/

$(function () {

    //绑定 页面
    $('#file').on('change', function () {
        var files = this.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var operate = new uploadOperate(file);
        }
    });

});
