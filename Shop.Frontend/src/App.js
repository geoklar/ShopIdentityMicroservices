import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Catalog } from './components/Catalog';
import { Cart } from './components/Cart';
import { Users } from './components/Users';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { AuthorizationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import { ApplicationPaths } from './components/Constants';

import './App.css'

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <AuthorizeRoute path={ApplicationPaths.CatalogPath} component={Catalog} />
        <AuthorizeRoute path={ApplicationPaths.CartPath} component={Cart} />
        <AuthorizeRoute path={ApplicationPaths.UsersPath} component={Users} />
        <Route path={AuthorizationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
      </Layout>
    );
  }
}
