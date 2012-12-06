define([
  'jquery',
  'underscore',
  'backbone',
  'models/test/TestModel'
], function ($, _, Backbone, TestModel) {
    var TestsCollection = Backbone.Collection.extend({
        model: TestModel,
        url: function () { return '/api/test'; },

        initialize: function (models, options) {
            //console.info('init coll');
        },

        parse: function (data) {
            //console.info('parse coll');
            return data;
        }
    });

    return TestsCollection;
});
