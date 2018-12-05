import React, {Component} from 'react';
import TorrentCard from "./TorrentCard/TorrentCard";
import {withStyles} from '@material-ui/core/styles';
import ProgressBar from "../../hocs/Layout/ProgressBar/ProgressBar";
import Infinite from 'react-infinite';
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
        , pageIndex: 0
        , pageCount: 1
        , torrents: []
    };

    setLoadingState = (isLoading) => this.setState({
        isLoading: isLoading
    });

    queryTorrents = (
        pageIndex
        , pageSize
    ) => {
        console.log(`Loading next page: ${pageIndex}`);

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
                        , pageIndex: pageIndex
                        , pageSize: pageSize
                    }
                }
            )
            .then(response => {
                this.setLoadingState(false);

                const data = response.data;

                return this.setState(previousSate => ({
                    torrents: [
                        ...previousSate.torrents
                        , ...data.items
                    ]
                    , pageIndex: previousSate.pageIndex + 1
                    , pageCount: data.pageCount
                }));
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
            , pageIndex: 0
            , pageCount: 1
        });

        this.queryTorrents(0, 10);
    }

    onInfiniteLoad = () => {
        if (!this.props.query
            || this.state.isLoading
            || this.state.pageIndex >= this.state.pageCount) {
            return;
        }

        this.queryTorrents(this.state.pageIndex, 10);
    };

    elementInfiniteLoad = () => {
        return <div className="infinite-list-item">
            Loading...
        </div>;
    };

    render = () => {
        const {classes} = this.props;

        return (
            <>
                <ProgressBar isLoading={this.state.isLoading}/>

                <div className={classes.search}>

                    <Infinite
                        useWindowAsScrollContainer={true}
                        elementHeight={200}
                        infiniteLoadBeginEdgeOffset={200}
                        onInfiniteLoad={this.onInfiniteLoad}
                        // loadingSpinnerDelegate={this.elementInfiniteLoad()}
                        isInfiniteLoading={this.state.isLoading}>
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
                    </Infinite>
                </div>
            </>
        );
    }
}


export default withStyles(styles)(Search);
