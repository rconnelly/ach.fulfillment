// Filename: router.js
define([
  'jquery',
  'underscore',
  'backbone',
  'views/home/HomeView',
  'views/projects/ProjectsView',
  'views/contributors/ContributorsView',
  'views/test/TestListView',
//'views/test/TestView',
  'views/footer/FooterView'
], function ($, _, Backbone, HomeView, ProjectsView, ContributorsView, TestListView, /*TestView,*/FooterView) {

    var AppRouter = Backbone.Router.extend({

        views: [],

        cleanViews: function () {
            if (this.views.length) {
                _.each(this.views, function (view) {
                    view.cleanUp();
                });
                this.views = [];
            }
        },

        routes: {
            // Define some URL routes
            'projects': 'showProjects',
            'users': 'showContributors',
            'tests': 'showTest',
            'tests/:id': 'editTest',

            // Default
            '*actions': 'defaultAction'
        }
    });

    var initialize = function () {

        var app_router = new AppRouter;

        app_router.on('route:showProjects', function () {

            // Call render on the module we loaded in via the dependency array
            var projectsView = new ProjectsView();
            projectsView.render();

        });

        app_router.on('route:showTest', function () {
            console.info('--> Routing to show tests');
            app_router.cleanViews();

            var testView = new TestListView({});

            app_router.views.push(testView);
        });

        app_router.on('route:editTest', function (id) {
            console.info('--> Routing to edit test ' + id);

            app_router.cleanViews();

            var testListView = new TestListView({ id: id });
            
            app_router.views.push(testListView);
        });

        app_router.on('route:showContributors', function () {

            // Like above, call render but know that this view has nested sub views which 
            // handle loading and displaying data from the GitHub API  
            var contributorsView = new ContributorsView();
        });

        app_router.on('route:defaultAction', function (actions) {

            // We have no matching route, lets display the home page 
            var homeView = new HomeView();
            homeView.render();

            // unlike the above, we don't call render on this view
            // as it will handle the render call internally after it
            // loads data 
            var footerView = new FooterView();

        });

        Backbone.history.start();
    };
    return {
        initialize: initialize
    };
});

