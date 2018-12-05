import React, {Component} from 'react';
import TorrentCard from "./TorrentCard/TorrentCard";
import {withStyles} from '@material-ui/core/styles';
import ProgressBar from "../../hocs/Layout/ProgressBar/ProgressBar";
import axios from 'axios';


const styles = theme => ({
    search: {
        padding: '16px'
        , maxWidth: '100%'
        , [theme.breakpoints.up('sm')]: {
            maxWidth: '700px'
            , margin: 'auto auto'
        },
    },
    torrentCard: {
        minWidth: '275px'
        , marginBottom: '16px'
    }
});

class Search extends Component {

    state = {
        isLoading: false
        , torrents: []
    };

    reduceTorrents = (responseData) => responseData
        .reduce((torrents, indexerResult) => torrents.concat(
            indexerResult.torrents.map(t => ({
                ...t
                , indexer: indexerResult.indexer
            })))
            , []
        )
        // Sort descending (this will eventually be done server side)
        .sort((a, b) => b.seeders - a.seeders);

    setLoadingState = (isLoading) => this.setState({
        isLoading: isLoading
    });

    queryTorrents = () => {
        if (typeof this.tokenSource !== typeof undefined) {
            this.tokenSource.cancel();
        }

        this.tokenSource = axios.CancelToken.source();

        this.setLoadingState(true);

        axios
            .get(
                this.props.endpoint
                , {
                    cancelToken: this.tokenSource.token
                    , params: {
                        queryString: this.props.query
                        , indexer: this.props.indexer
                    }
                }
            )
            .then(response => {
                this.setLoadingState(false);

                return this.setState({
                    torrents: response.data.items
                });
            })
            .catch(error => {
                if (axios.isCancel(error)) {
                    return;
                }

                this.setLoadingState(false);

                return this.props.onError(`Query Failed: ${error.message}`);
            });
    };

    componentDidUpdate(prevProps, prevState, snapshot) {
        // Prevent fetching the data multiple times.
        if (this.props.query === prevProps.query) {
            return;
        }

        this.setState({
            torrents: []
        });

        this.queryTorrents();
    }

    render = () => {
        const {classes} = this.props;

        return (
            <>
                <ProgressBar isLoading={this.state.isLoading}/>

                <div className={classes.search}>
                    {this.state.torrents.map((torrent, index) => (
                        <TorrentCard
                            key={index}
                            className={classes.torrentCard}
                            name={torrent.name}
                            magnetUri={torrent.magnetUri}
                            uploadedOn={torrent.uploadedOn}
                            size={torrent.size}
                            seeders={torrent.seeders}
                            leechers={torrent.seeders}/>
                    ))}
                </div>
            </>
        );
    }
}


export default withStyles(styles)(Search);
