define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var TestModel = Backbone.Model.extend({
        defaults: {
            name: 'default name'
        },
        urlRoot: '/api/test',
        
        initialize: function () {
            this.on("change:name", function (model) {
                //var name = model.get("name");
            });
        }
    });

    return TestModel;
});
