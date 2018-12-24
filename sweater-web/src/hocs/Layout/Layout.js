import React, {Component} from 'react';
import {withStyles} from '@material-ui/core/styles';
import SearchAppBar from "./SearchAppBar/SearchAppBar";
import Search from "../../containers/Search/Search";
import ResultSnackbar from "../../containers/ResultSnackbar/ResultSnackbar";
import Theme from "./Theme/Theme";
import MuiThemeProvider from "@material-ui/core/es/styles/MuiThemeProvider";

// TODO: Move these to some configuration
const baseEndpoint = `https://localhost:8080/api`;
const pageSize = 10;
const indexer = 'all';

const styles = theme => ({
    // Use this style to align content below fixed App Bar.
    toolbar: theme.mixins.toolbar
});

class Layout extends Component {

    state = {
        query: null
    };

    constructor(props) {
        super(props);

        this.resultSnackbar = React.createRef();
    }

    onSearch = (query) => this.setState({
        query: query
    });

    showError = (message) => {
        const resultSnackbar = this
            .resultSnackbar
            .current;

        if (resultSnackbar == null) {
            return;
        }

        resultSnackbar.showError(message);
    };

    render() {
        const {classes} = this.props;

        return (
            <MuiThemeProvider theme={Theme}>
                <SearchAppBar
                    title={'Sweater'}
                    onSearch={this.onSearch}/>

                <div className={classes.toolbar}/>

                <Search
                    endpoint={`${baseEndpoint}/indexer/query`}
                    pageSize={pageSize}
                    indexer={indexer}
                    query={this.state.query}
                    onError={this.showError}/>

                <ResultSnackbar ref={this.resultSnackbar}/>
            </MuiThemeProvider>
        );
    }
}

export default withStyles(styles)(Layout);
