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
            },

            load: function (options) {
                var that = this;
                var onDataHandler = function (collection) {
                    console.info('#update collection. new len ' + collection.length);
                    that.collection = collection;
                    that.render();

                    if (options.id != undefined) {
                        console.info('add/edit id ' + options.id);
                        var testView = new TestView({ el: $("#edit-form"), id: options.id, p: that });
                    }
                };

                console.info('#init collection.');

                this.collection.fetch({ success: onDataHandler, dataType: "json" });
            },

            del: function (e) {
                e.stopPropagation();

                var caller = e.target || e.srcElement;
                var id = $(caller).attr("data-id");

                console.info('deleteing ' + id + " col len " + this.collection.length);

                var m = this.collection.get(id);

                if (m == undefined) {
                    alert('ERROR');
                    return;
                }

                //debugger;
                console.info('deleteing ' + m.get('name'));

                var that = this;
                m.destroy({
                    success: function (model, response) {
                        that.collection.remove(id);
                        that.render();
                        //Backbone.history.navigate('/#tests');
                    }
                });
            },

            render: function () {
                $('.menu li').removeClass('active');
                $('.menu li a[href="#/tests"]').parent().addClass('active');

                var data = { tests: this.collection.toJSON() };
                var compiledTemplate = _.template(testListTemplate, data);
                this.$el.html(compiledTemplate);

                // this.delegateEvents();
            }
        });

        return TestListView;
    });
