define([
        'jquery',
        'underscore',
        'backbone',
        'views/sidebar/SidebarView',
        'models/test/TestModel',
        'text!templates/test/testTemplate.html'
    ], function ($, _, Backbone, SidebarView, TestModel, testTemplate) {

        var TestView = Backbone.View.extend({

            initialize: function (options) {
                this.render(options.id);
            },

            events: {
                "click #save": "doSave"
            },

            doSave: function (event) {
                console.info('saving model');

                // todo: is there better way to update model? knockout?
                var that = this;
                this.model.save(
                    { name: $('#name').val() },
                    {
                        success: function (m) {
                            console.info('rasing updated event for '+ m.id + '....');
                            that.trigger('updated', m);
                        }
                    }
                );
            },

            render: function (id) {
                var that = this;
                console.info('render edit ' + id);

                this.model = new TestModel();

                var compiledTemplate = _.template(testTemplate);
                var data = { test: this.model.toJSON() };

                if (id > 0) {
                    this.model.set('id', id);
                    this.model.fetch({
                        success: function (m) {
                            console.info('got model from server');

                            data.test = m.toJSON();
                            that.$el.html(compiledTemplate(data));
                        }
                    });
                } else {
                    that.$el.html(compiledTemplate(data));
                }
            }
        });

        return TestView;
    });
