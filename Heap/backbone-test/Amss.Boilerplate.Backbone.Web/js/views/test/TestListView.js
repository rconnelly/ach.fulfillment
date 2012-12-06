define([
        'jquery',
        'underscore',
        'backbone',
        'collections/test/TestsCollection',
        'models/test/TestModel',
        'views/test/TestView',
        'text!templates/test/testListTemplate.html'
    ], function ($, _, Backbone, TestsCollection, TestModel, TestView, testListTemplate) {

        var TestListView = Backbone.View.extend({
            el: $("#page"),

            collection: new TestsCollection,

            events: {
                "click .del": "del"
            },

            initialize: function (options) {
                this.load(options);

                _.bindAll(this, 'up');
            },

            childeView: null,

            load: function (options) {
                console.info('loading tests....');

                var that = this;
                var onDataHandler = function (collection) {
                    console.info('tests loaded. count ' + collection.length);
                    that.collection = collection;
                    that.render();

                    if (options.id != undefined) {
                        console.info('add/edit id ' + options.id);
                        
                        var data = { el: $("#edit-form"), id: options.id};
                        that.childeView = that.childeView ? that.childeView(data) : new TestView(data);
                        that.childeView.bind('updated', that.up);
                    }
                };

                this.collection.fetch({ success: onDataHandler, dataType: "json" });
            },

            up: function (m) {
                console.info('handle updated event....');
                // this.collection.add(m);
                Backbone.history.navigate('/#tests');
            },

            del: function (e) {
                e.stopPropagation();

                var caller = e.target || e.srcElement;
                var id = $(caller).attr("data-id");

                console.info('deleting test ' + id);

                var m = this.collection.get(id);

                if (m == undefined) {
                    console.info('!!!can not find test ' + id + '. tests count ' + this.collection.length);
                    return;
                }

                //debugger;

                var that = this;
                m.destroy({
                    success: function (model, response) {
                        console.info('test ' + id + ' has been deleted');
                        that.collection.remove(id);
                        that.render();
                    }
                });
            },

            render: function () {
                console.info('rendering tests...');

                $('.menu li').removeClass('active');
                $('.menu li a[href="#/tests"]').parent().addClass('active');

                var data = { tests: this.collection.toJSON() };
                var compiledTemplate = _.template(testListTemplate, data);
                this.$el.html(compiledTemplate);
            },

            cleanUp: function () {
                console.info('cleaning up view');
                this.undelegateEvents();
            }
        });

        return TestListView;
    });
